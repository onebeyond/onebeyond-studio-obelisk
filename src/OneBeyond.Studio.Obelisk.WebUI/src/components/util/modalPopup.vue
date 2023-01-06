<template>
    <div class="modal-mask" v-if="visible">
        <div
            class="modal"
            style="display:block"
            tabindex="-1"
            role="dialog"
            id="modalPopup"
        >
            <div :class="['modal-dialog', modalClass]" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <slot name="header">
                            <h5 class="modal-title">{{innerTitle}}</h5>
                            <button type="button" class="close" aria-label="Close" @click="closeClick">
                                <span aria-hidden="true">
                                    &times;
                                </span>
                            </button>
                        </slot>
                    </div>

                    <div class="modal-body">
                        <slot name="content">
                            <p>{{innerMessage}}</p>
                        </slot>
                    </div>

                    <div class="modal-footer">
                        <slot name="footer">
                            <button type="button" class="btn btn-secondary" @click="closeClick">
                                {{$t('button.close')}}
                            </button>
                        </slot>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>


<script lang="ts">
    import Component from "vue-class-component";
    import {Vue, Prop } from "vue-property-decorator";

    @Component({
        name: "modalPopup",
        components: {}
    })
    export default class modalPopup extends Vue {
        @Prop({ type: Object }) bus!: any;
        @Prop({ type: String }) namespace!: string;
        @Prop({ type: Boolean }) visible!: boolean;
        @Prop() message!: string;
        @Prop() title!: string;
        @Prop() modalClass!: string;

        innerTitle: string;
        innerMessage: string;

        constructor() {
            super();
            this.innerTitle = "";
            this.innerMessage = "";
        }

        created(): void {
            this.innerTitle = this.title;
            this.innerMessage = this.message;

            if (this.bus && this.namespace) {
                this.bus.$on(this.namespace + "/setData", this.setData);
            }
        }

        closeClick(): void {
            const self = this;
            const selfBase = self as any;
            //notify that modal wants to be closed
            //prop.visible should be changed in the parent to actually close the modal
            if (this.bus && this.namespace) {
                this.bus.$emit(this.namespace + "/close");
            }
            selfBase.$emit("close");
        }
        setData(title: string, msg: string): void {
            this.innerTitle = title;
            this.innerMessage = msg;
        }
    }
</script>
