import { plainToInstance } from "class-transformer";

import { Entity } from "@js/dataModels/entity";
import DcslResourceApiClient from "@js/api/dcslResourceApiClient";

import type { EntityBuilder } from "@js/dataModels/entity";

export default class EntityApiClient<TEntity extends Entity<T>, T> extends DcslResourceApiClient {

    constructor(
        private readonly entityBuilder: EntityBuilder<TEntity, T>,
        resource: string,
        version: string | null
    ) {
        super(resource, version);
    }

    public get apiUrl(): string {
        return this.apiBaseUrl;
    }

    public async getEntity(id: T): Promise<TEntity> {
        const data = await this.get(`${id}`);
        return plainToInstance(this.entityBuilder, await data.json());
    }

    public async addEntity(entity: TEntity): Promise<T> {
        const data = await this.post("", entity);
        return (await data.json()) as T; //returns new entity Id
    }

    public async updateEntity(entity: TEntity): Promise<void> {
        await this.put(`${entity.id}`, entity);
    }

    public async deleteEntity(id: T): Promise<void> {
        await this.delete(`${id}`);
    }
}