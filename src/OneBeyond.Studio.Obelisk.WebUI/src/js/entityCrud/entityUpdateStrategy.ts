import EntityCrudMixin from "@js/entityCrud/entityCrudMixin";

import { Entity } from "@js/dataModels/entity";
import { EntityGrid, EntityGridAction } from "@js/grids/entityGrid";

//This is the base class for entity create/update behaviour. You can create your own versions of create/update behaviour deriving from it.
export abstract class EntityUpdateStrategy<TEntity extends Entity<T>, T, TGrid extends EntityGrid> {
    public abstract doAdd(caller: EntityCrudMixin<TEntity, T, TGrid>): void; //Reaction to user clicked on Add entity button
    public abstract doEdit(caller: EntityCrudMixin<TEntity, T, TGrid>, entityId: T): Promise<void>; //Reaction to user clicked on Edit entity button
    public abstract doViewDetails(caller: EntityCrudMixin<TEntity, T, TGrid>, entityId: T): void; //Reaction to user clicked on View entity details button
    public abstract doEntitySaved(caller: EntityCrudMixin<TEntity, T, TGrid>): void; //Reaction on entity saved
    public abstract doReturn(caller: EntityCrudMixin<TEntity, T, TGrid>); //Reaction on "Back"button
}

//Add/Update entity in a modal
export class EntityUpdateUsingModal<TEntity extends Entity<T>, T, TGrid extends EntityGrid> extends EntityUpdateStrategy<TEntity, T, TGrid> {

    public doAdd(caller: EntityCrudMixin<TEntity, T, TGrid>): void {
        caller.entityGrid.rememberCurrentPageBeforeGridAction(EntityGridAction.EntityAdd);
        caller.entity = new (caller.provideEntityBuilder())();
        caller.onEntityLoaded();
        caller.showEntity = true;
    }

    public async doEdit(caller: EntityCrudMixin<TEntity, T, TGrid>, entityId: any): Promise<void> {
        caller.entityGrid.rememberCurrentPageBeforeGridAction(EntityGridAction.EntityEdit);

        try {
            caller.entity = await caller.entityApiClient.getEntity(entityId);
            caller.onEntityLoaded();
            caller.showEntity = true;
        }
        catch (e) {
            caller.onError(e);
        }
    }
    
    public doViewDetails(caller: EntityCrudMixin<TEntity, T, TGrid>, entityId: any): void { // eslint-disable-line @typescript-eslint/no-unused-vars
        //Do nothing? Or the reaction should be the same as doEdit?
    }

    public doEntitySaved(caller: EntityCrudMixin<TEntity, T, TGrid>): void {
        caller.onEntityUpdated();
    }

    public doReturn(caller: EntityCrudMixin<TEntity, T, TGrid>) { // eslint-disable-line @typescript-eslint/no-unused-vars
        //Do nothing? Or should we close the add/edit modal window here?
    }

}

//Add/Update entity on a separate page
export class EntityUpdateUsingSeparatePage<TEntity extends Entity<T>, T, TGrid extends EntityGrid> extends EntityUpdateStrategy<TEntity, T, TGrid> {

    public readonly addView: string;
    public readonly editView: string;
    public readonly detailsView: string;
    public readonly listView: string;

    constructor(
        addView: string,
        editView: string,
        detailsView: string,
        listView: string
    ) {
        super();

        this.addView = addView;
        this.editView = editView;
        this.detailsView = detailsView;
        this.listView = listView;
    }

    public doAdd(caller: EntityCrudMixin<TEntity, T, TGrid>): void {
        caller.$router.push({ name: this.addView, params: { id: caller.provideEntityBuilder().idDefault() as any } });
    }

    public async doEdit(caller: EntityCrudMixin<TEntity, T, TGrid>, entityId: any): Promise<void> {
        caller.$router.push({ name: this.editView, params: { id: entityId } });
    }

    public doViewDetails(caller: EntityCrudMixin<TEntity, T, TGrid>, entityId: any): void {
        caller.$router.push({ name: this.detailsView, params: { id: entityId } });
    }

    public doEntitySaved(caller: EntityCrudMixin<TEntity, T, TGrid>): void {
        this.doReturn(caller);
    }

    public doReturn(caller: EntityCrudMixin<TEntity, T, TGrid>) {
        caller.$router.push({ name: this.listView });
    }
}

//Add/Update entity in a grid inline
export class EntityUpdateUsingInlineGrid<TEntity extends Entity<T>, T, TGrid extends EntityGrid> extends EntityUpdateUsingModal<TEntity, T, TGrid> {

    public doAdd(caller: EntityCrudMixin<TEntity, T, TGrid>): void {
        super.doAdd(caller);
        caller.isEditingEntityInline = !caller.isMobile();
    }

    public async doEdit(caller: EntityCrudMixin<TEntity, T, TGrid>, entityId: any): Promise<void> {
        super.doEdit(caller, entityId);
        caller.isEditingEntityInline = !caller.isMobile();
    }
}
