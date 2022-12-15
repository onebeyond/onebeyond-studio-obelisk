import { Component, Mixins } from "vue-property-decorator";

import { EntityGrid, EntityGridAction } from "@js/grids/entityGrid";
import { EntityUpdateStrategy, EntityUpdateUsingModal } from "@js/entityCrud/entityUpdateStrategy";

import { Entity, EntityBuilder } from "@js/dataModels/entity";
import AlertMixin from "@js/mixins/alertMixin";
import LookupMixin from "@js/mixins/lookupMixin";
import EntityApiClient from "@js/api/entityApiClient";

//Note, TEntity is the entity we want to manage, T is the type of the entity Id
@Component
export default class EntityCrudMixin<TEntity extends Entity<T>, T, TGrid extends EntityGrid> extends Mixins(AlertMixin, LookupMixin) {

    public entityGrid: TGrid; //All settings for the entity grid 
    public entityApiClient!: EntityApiClient<TEntity, T>; //All communication to the server web API should be via EntityDataManager
    public entity: TEntity; //An entity being added/edited

    //Note! There is no more this.isModal! 
    //You can define how your page is going to react on add/edit 
    //(create an entity in a modal or on a sep page) using EntityUpdateStrategy class
    public entityUpdateStrategy: EntityUpdateStrategy<TEntity, T, TGrid>;

    public isSaving: boolean = false;
    public isLoading: boolean = false;
    public isEditingEntityInline: boolean;
    public showEntity: boolean;
    public showDeleteEntity: boolean;

    constructor() {
        super();

        this.entity = new (this.provideEntityBuilder())();

        //Note! We create entity grid on component created! 
        //Here we just indicate to Vue js that all the properties of the grid need to be reactive, using this trick. 
        this.entityGrid = {} as any;

        this.entityUpdateStrategy = new EntityUpdateUsingModal<TEntity, T, TGrid>(); //by default we update entity using modal window

        this.isEditingEntityInline = false;
        this.showEntity = false;
        this.showDeleteEntity = false;
    }

    created(): void {
        // Note, we create here an instance of apiClient which will be responsible to make the HTTP requests to the server
        this.entityApiClient = this.provideEntityApiClient();

        //Note, we create a grid here, not in the constructor, as if we create it in the constructor, all component properties will be unavailable in this.provideGrid.
        this.entityGrid = this.provideGrid();
        this.entityGrid.initDataAdaptor(this.provideGridUrl(), this.onError);
    }

    mounted(): void {
        const gridReference = this.getGridReference(this);

        if (gridReference == null) {
            console.log("No grid reference found")
        }
        else {
            //the problem is $refs are instantiated when the component is mounted, we can't do it in a constructor 
            //this may need to be changed, what if we use multiple grids on the same page?
            this.entityGrid.setInstance(gridReference);
        }
    }
    private getGridReference(vueComponentInstance: any): any {
        let instance = null;
        //Top level grid
        if (vueComponentInstance.$refs.entityGrid) {
            instance = vueComponentInstance.$refs.entityGrid
        }
        else {
            //Wrapped / nested grid
            for (const childInstance of vueComponentInstance.$children) {
                instance = this.getGridReference(childInstance);
                if (instance != null) {
                    break;
                }
            }
        }
        return instance;
    }

    //Important Note! Every class that uses EntityCrudMixin needs to provide an entity builder.
    //This is actually a class, which constructor will be used to create a new instance of the entity.
    //So in case if your entity is
    //
    //   this.entity: Product,
    //
    //provideEntityBuilder should return Product class:
    //
    //   protected provideEntityBuilder(): EntityBuilder<Product, string> { return Product; }
    //
    //The unusual way this method is implemented is because we need to use both EntityBuilder
    //in the constructor, and we can't pass those values as parameters into the constructor
    public provideEntityBuilder(): EntityBuilder<TEntity, T> {
        throw Error("Please provide entity builder to instantiate entity data manager");
    }

    //Important Note! Every class that uses EntityCrudMixin needs to provide an entity client.
    public provideEntityApiClient(): EntityApiClient<TEntity, T> {
        throw Error("Please provide entity api client");
    }

    // Can ovverride in case of a custom url needed
    public provideGridUrl(): string {
        return this.entityApiClient.apiUrl;
    }

    //This isn't ideal because it means we have to provide a grid on every page we use the mixin
    //e.g
    //provideGrid(): TGrid {
    //    return new VueEntityGrid([]);
    //}
    provideGrid(): TGrid {
        throw Error("Please provide entity grid");
    }

    //Note! addClick and addEntity are both moved to addClick.
    //entityUpdateStrategy will define how to react on a "Add Entity" button click
    onAddEntityButtonClicked(): void {
        this.entityUpdateStrategy.doAdd(this);
    }

    async onEditEntityButtonClicked(id: T): Promise<void> {
        return this.entityUpdateStrategy.doEdit(this, id);
    }

    onViewEntityDetailsButtonClicked(id: any): void {
        this.entityUpdateStrategy.doViewDetails(this, id);
    }

    onDeleteEntityButtonClicked(id: T): void {
        this.entityGrid.rememberCurrentPageBeforeGridAction(EntityGridAction.EntityDelete);
        this.showDeleteEntity = true;
        this.entity.id = id;
    }

    onDeleteEntityCancelled(): void {
        this.showDeleteEntity = false;
    }

    //Note, this is former returnToList button
    onBackButtonClicked(): void {
        this.entityUpdateStrategy.doReturn(this);
    }

    closeEntityModal(): void {
        this.showEntity = false;
        this.isEditingEntityInline = false;
    }

    onEntityLoaded(): void {
        //This event handler will be called after:
        // (1) a new entity is created(when we add a new entity)
        // (2) an entity is retrieved from server (when we edit an entity)
        //Please override it if you need to perform any additional operations on this event
    }

    onEntityUpdated(): void {
        //This event handler will be called after an entity (a new or an existing one) is sucessfully saved on server.
        this.entityGrid.restoreCurrentPage();
        this.entity = new (this.provideEntityBuilder())();
        this.showEntity = false;
        this.isEditingEntityInline = false;
    }

    onEntityDeleted(): void {
        //This event handler will be called after an entity is sucessfully deleted on server.
        this.entityGrid.restoreCurrentPage();
    }

    async fetchData(id: T): Promise<void> {
        try {
            this.isLoading = true;
            this.entity = await this.entityApiClient.getEntity(id);
            this.onEntityLoaded();
        }
        catch (e) {
            this.onError(e);
        }
        finally {
            this.isLoading = false;
        }
    }

    async saveEntity(): Promise<void> {
        this.entity.trimProperties();

        // Revalidate the whole form, in case no focus on inputs occurred. 
        // Save button will be auto-disabled if any errors are found
        const validationPassed = await this.$validator.validateAll();

        if (validationPassed) {
            try {
                this.isSaving = true;
                this.entity.isNew
                    ? this.entity.id = await this.entityApiClient.addEntity(this.entity)
                    : await this.entityApiClient.updateEntity(this.entity);

                this.entityUpdateStrategy.doEntitySaved(this);
            }
            catch (e) {
                this.onError(e);
            }
            finally {
                this.isSaving = false;
            }
        }
    }

    async deleteEntity(): Promise<void> {
        this.showDeleteEntity = false;

        try {
            await this.entityApiClient.deleteEntity(this.entity.id);
            this.onEntityDeleted();
        }
        catch (e) {
            this.onError(e);
        }
    }

    onError(error: any): void {
        let msg = "Error Processing Request";
        if (!!error && !!error.toString()) msg = msg + ": " + error.toString();
        this.showAlert("Error", msg);
    }

    //This method returns true is a page is displayed on a modal device (or a small resolution screen)
    public isMobile(): boolean {
        const isMobileCheckDiv = document.getElementById('isMobileCheckDiv') as Element;

        if (isMobileCheckDiv === null) {
            throw new Error("Unable to detect if the app is in mobile mode: cannot find div#isMobileCheckDiv");
        }

        return window.getComputedStyle(isMobileCheckDiv).display !== 'none';
    }

}
