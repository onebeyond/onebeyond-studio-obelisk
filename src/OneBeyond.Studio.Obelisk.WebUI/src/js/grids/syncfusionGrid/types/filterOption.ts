import { FilterOperator } from "./types";

export class FilterOption {
    constructor(public value: FilterOperator, public text: string) {
    }
}