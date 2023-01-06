import { showYesNo, currency, sizeInKb } from "@js/util/filters";

// TODO: From Sligo - there's definitely a good chance this code is redundant. Did it get copied from somewhere.
// TODO: I don't know where to log code issues - so have left them in TODOs. If there's a better place, please let me know.
export default class CommonDataAccessors {
    static showYesNo(field: string, data: any, column: any) { // eslint-disable-line @typescript-eslint/no-unused-vars
        return showYesNo(data[field]);
    }

    static currency(field: string, data: any, column: any, symbol, decimals, options) { // eslint-disable-line @typescript-eslint/no-unused-vars
        return currency(data[field], symbol, decimals, options)
    }

    static sizeInKb(field: string, data: any, column: any) { // eslint-disable-line @typescript-eslint/no-unused-vars
        return sizeInKb(data[field]);
    }

    static dataAccessorAdapter(filter: any, ...parameters: any): any {
        return function (field, data, column): any { // eslint-disable-line @typescript-eslint/no-unused-vars
            return filter(data[field], parameters);
        }
    }
}

export const SyncfusionShowYesNo = CommonDataAccessors.showYesNo;
export const SyncfusionCurrency = CommonDataAccessors.currency;
export const SyncfusionSizeInKb = CommonDataAccessors.sizeInKb;
export const DataAccessorAdapter = CommonDataAccessors.dataAccessorAdapter;
