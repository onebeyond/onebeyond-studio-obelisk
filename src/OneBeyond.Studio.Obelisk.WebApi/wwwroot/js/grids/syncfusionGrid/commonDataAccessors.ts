import { showYesNo, currency, sizeInKb } from "@js/util/filters";

export default class CommonDataAccessors {
    static showYesNo(field: string, data: any, column: any) {
        return showYesNo(data[field]);
    }

    static currency(field: string, data: any, column: any, symbol, decimals, options) {
        return currency(data[field], symbol, decimals, options)
    }

    static sizeInKb(field: string, data: any, column: any) {
        return sizeInKb(data[field]);
    }

    static dataAccessorAdapter(filter: any, ...parameters: any): any {
        return function (field, data, column): any {
            return filter(data[field], parameters);
        }
    }
}

export const SyncfusionShowYesNo = CommonDataAccessors.showYesNo;
export const SyncfusionCurrency = CommonDataAccessors.currency;
export const SyncfusionSizeInKb = CommonDataAccessors.sizeInKb;
export const DataAccessorAdapter = CommonDataAccessors.dataAccessorAdapter;
