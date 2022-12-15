export abstract class Entity<T> {

    id: T;

    constructor(id: T) {
        this.id = id;
    }

    abstract get isNew(): boolean;

    public trimProperties(): void {
        const obj = this as object;
        for (const key in obj) {
            if (typeof obj[key] === "string" && Object.getOwnPropertyDescriptor(obj, key)) {
                obj[key] = obj[key].trim();
            }
        }
    }
}

export class IntEntity extends Entity<number> {

    constructor() {
        super(0)
    }

    get isNew(): boolean {
        return this.id === IntEntity.idDefault();
    }

    //default id value for this type of id
    public static idDefault(): number {
        return 0;
    }
}

export class GuidEntity extends Entity<string> {

    constructor() {
        super("")
    }

    get isNew(): boolean {
        return this.id === GuidEntity.idDefault();
    }

    //default id value for this type of id
    public static idDefault(): string {
        return "";
    }

}

//That's the way we provide access to entity's constructor, pleease see entityCrudMixin -> provideEntityBuilder
export interface EntityBuilder<TEntity extends Entity<T>, T> {
    new(...args: any[]): TEntity;
    idDefault(): T;
}
