import Vue from 'vue'

declare module "vue/types/vue" {
    /**
     * NOTE: this should be use only to extend Vue functionality, NOT
     * for agnostic appSettings. For configuration settings please populate 
     * the object/s located in @js/dataModels/settings
     */
    interface Vue {
        $buildNumber: string;
        $buildDate: string;
        $rootPath: string;
    }
}
