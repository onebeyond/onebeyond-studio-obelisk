import Vue from "*.vue";
import
{ VueConstructor }
    from "vue/types/umd";

export interface LocalVueComponent
{
    name?: string,
    template?: string,
    data: () => any,
    methods?: any,
    computed?: any,
    components?: any,
    extends?: VueConstructor<Vue>
}
