import Vue from "*.vue";
import
{ VueConstructor }
    from "vue/types/umd";

export interface LocalVueComponent
{
    name?: string,
    template?: string,
    data: () => {},
    methods?: {},
    computed?: {},
    components?: {},
    extends?: VueConstructor<Vue>
}