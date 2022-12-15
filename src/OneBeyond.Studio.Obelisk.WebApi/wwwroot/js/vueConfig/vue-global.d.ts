import Vue from 'vue'

declare module "vue/types/vue" {
    interface Vue {
        $rootPath: string;
        $rootApiPath: string;
        $createLookup: any;
        $moment: any;
    }
}
