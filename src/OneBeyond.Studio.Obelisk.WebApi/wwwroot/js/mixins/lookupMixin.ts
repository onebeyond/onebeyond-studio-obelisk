import Vue from "vue";
import { Component } from "vue-property-decorator";
import LookupFetcher from "@js/util/lookupFetcher";

@Component
export default class LookupMixin extends Vue {
    lookupNames: any[];
    lookupFetchers: Map<string, LookupFetcher>;
    lookups: any;

    constructor() {
        super();

        this.lookupNames = [];
        this.lookupFetchers = new Map<string, LookupFetcher>();
        this.lookups = {};
    }

    public initLookups(arg: any): void {
        let lookupsArray: any[] = Array.isArray(arg)
            ? arg
            : [arg];
        lookupsArray.forEach((lookupName: any) => {
            if (lookupName === Object(lookupName)) lookupName = lookupName.lookup;
            this.lookupNames.push(lookupName);
            this.lookupFetchers[lookupName] =
                new LookupFetcher(this)
                    .setRootName("lookups") // NOTE: this must match the name of the lookups property
                    .setName(lookupName)
                    .setEndpoint(lookupName);
        });
    }

    public async fetchLookups(): Promise<void> {
        for (let lookupName of this.lookupNames) {
            await this.lookupFetchers[lookupName].executeFetch();
        }
    }
}
