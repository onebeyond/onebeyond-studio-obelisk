import { plainToInstance } from "class-transformer";
import * as Vue from "vue/types/umd";
import DcslApiClient from "@js/api/dcslApiClient";

/**
 * This class replaces the previous JS npm package
 * Use it to fetch static lookups from your WebApi
 */
export default class LookupFetcher extends DcslApiClient {
    private idPropName: string = 'id';
    private rootAttributeName: string = 'lookups';
    private name: string = 'Lookup';
    private apiEndpoint: string = '';
    private urlParams: any = null;
    private fields: string[] = ['id', 'name'];
    private failureCallback: Function | null = null;
    private afterFetchCallback: Function | null = null;
    private vueInstance: Vue;

    constructor(vueInstance: Vue) {
        super(`${(window as any).BaseUrl}api`);
        this.vueInstance = vueInstance;
    }

    public setRootName(rootAttributeName: string): LookupFetcher {
        this.rootAttributeName = rootAttributeName;
        return this;
    }

    public setIdPropName(idPropName: string): LookupFetcher {
        this.idPropName = idPropName;
        return this;
    }

    public setName(name: string): LookupFetcher {
        this.name = name;
        return this;
    }

    public setEndpoint(apiEndpoint: string, urlParams?: string): LookupFetcher {
        if (!apiEndpoint.startsWith("/")) {
            apiEndpoint = `/${apiEndpoint}`;
        }

        if (!apiEndpoint.endsWith("/")) {
            apiEndpoint += "/";
        }

        this.apiEndpoint = apiEndpoint;
        this.urlParams = urlParams ?? "";
        return this;
    }

    public setFields(fields: any): LookupFetcher {
        this.fields = fields;
        return this;
    }

    public addField(field: string): LookupFetcher {
        this.fields.push(field);
        return this;
    }

    public setFailureCallback(callback: Function): LookupFetcher {
        this.failureCallback = callback;
        return this;
    }

    public setAfterFetchCallback(callback: Function): LookupFetcher {
        this.afterFetchCallback = callback;
        return this;
    }

    public async executeFetch(): Promise<LookupFetcher> {
        if (!this.vueInstance[this.rootAttributeName]) {
            this.vueInstance[this.rootAttributeName] = {};
        }

        let query = Object.keys(this.urlParams)
            .map(k => `${encodeURIComponent(k)}=${encodeURIComponent(this.urlParams[k])}`)
            .join('&');

        if (query !== '') query = '?' + query;

        try {
            const response = await this.get(`${this.apiEndpoint}${query}`);
            const body = await response.json();

            this.vueInstance.$set(this.vueInstance[this.rootAttributeName], this.name, []);
            this.onReadSuccess(body);
        }
        catch (error: any) {
            this.onReadFailure(error);
        }

        return this;
    }

    public getAll(): any {
        return this.vueInstance[this.rootAttributeName][this.name];
    }

    public findById(id: any): any {
        if (this.vueInstance[this.rootAttributeName][this.name]) {
            return this.vueInstance[this.rootAttributeName][this.name].find((item) => item[this.idPropName] === id);
        }
        return '';
    }

    private onReadFailure(error: any): void {
        if (this.failureCallback) {
            this.failureCallback(error);
        }
    }

    private onReadSuccess(body: any): void {
        this.vueInstance[this.rootAttributeName][this.name] = [];
        let data = body.data ? body.data : body;

        for (var i in data) {

            let obj = {};
            this.fields.forEach(function (i) { obj[i] = null; });
            obj = plainToInstance(Object, data[i]);

            this.vueInstance[this.rootAttributeName][this.name].push(obj);
        }

        this.vueInstance.$forceUpdate();

        // Call AfterFetch
        if (this.afterFetchCallback) {
            this.afterFetchCallback(this.vueInstance[this.rootAttributeName][this.name]);
        }
    }
}