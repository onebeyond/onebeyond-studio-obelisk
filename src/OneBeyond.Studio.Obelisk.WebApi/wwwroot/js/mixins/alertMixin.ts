import Vue from "vue";
import { Component } from "vue-property-decorator";

@Component
export default class AlertMixin extends Vue {
    alertVisible: boolean;

    constructor() {
        super();
        
        this.alertVisible = false;
    }

    showAlert(title: string, msg: string): void {
        this.$nextTick(() => { this.$emit("alertModal/setData", title, msg); });
        this.alertVisible = true;
    }

    hideAlert(): void {
        this.alertVisible = false;
    }
}
