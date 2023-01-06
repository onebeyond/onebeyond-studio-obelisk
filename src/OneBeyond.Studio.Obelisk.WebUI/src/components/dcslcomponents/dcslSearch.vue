<template>
    <div>
        <div v-if="modal">
            <v-modalPopup
                :bus="this"
                :namespace="'modal'"
                :visible="modalVisible"
                @close="modalVisible = false"
            >
                <template slot="content">
                    <slot>
                        <v-server-table :columns="columns" :options="options" ref="entityTable">
                            <div slot="Actions" slot-scope="props">
                                <button
                                    id="editBtn"
                                    class="btn btn-primary btn-sm"
                                    @click="selectClick(props.row)"
                                >
                                    {{$t('button.select')}}
                                </button>
                            </div>
                        </v-server-table>
                    </slot>
                </template>
            </v-modalPopup>
        </div>
        <div v-else>
            <slot>
                <v-server-table
                    :url="apiUrl"
                    :columns="columns"
                    :options="options"
                    ref="entityTable"
                >
                    <div slot="Actions" slot-scope="props">
                        <button
                            id="editBtn"
                            class="btn btn-primary btn-sm"
                            @click="selectClick(props.row)"
                        >
                            {{$t('button.select')}}
                        </button>
                    </div>
                </v-server-table>
            </slot>
        </div>
    </div>
</template>

<script lang="ts">
    import { Vue, Component, Prop } from "vue-property-decorator";
    import { DataAdaptor } from "@js/grids/vueServerGrid/dataAdaptor";

    @Component({
        name: "DcslSearch",
        components: {}
    })
    export default class DcslSearch extends Vue {
        @Prop({ required: false }) value!: any;
        @Prop({ required: true, type: Object }) schema!: any;
        @Prop({ required: true, type: String }) apiUrl!: string;
        @Prop({ required: false, type: Boolean }) modal!: boolean;
        @Prop({ required: false }) gridOptions!: any;

        public modalVisible: boolean;
        private dataAdaptor: DataAdaptor | null;

        constructor() {
            super();
            this.modalVisible = false;
            this.dataAdaptor = null;
        }

        created(): void {
            this.dataAdaptor = new DataAdaptor(this.apiUrl, this.onError);
        }

        //computed
        get columns(): string[] {
            var keys = Object.keys(this.schema);
            keys.push("Actions");
            return keys;
        }

        get options(): any {
            return this.gridOptions ? this.gridOptions : // if gridOptions is not set, we use the default one
                {
                    perPage: 5,
                    perPageValues: [],
                    pagination: { dropdown: true },
                    filterByColumn: false,
                    filterable: Object.keys(this.schema), // all columns are set as filterable
                    texts: {
                        filter: "",
                        filterPlaceholder: this.$t('placeholder.search'),
                    },
                    headings: this.headings,
                    requestFunction: (params) => this.dataAdaptor!.executeApi(params)
                }
        }

        get headings(): any {
            const headings = {};
            Object.keys(this.schema).forEach(key => {
                if (this.schema[key]) {
                    headings[key] = this.schema[key];
                }
            })
            return headings;
        }

        //methods
        show(): void {
            this.modalVisible = true;
        }

        selectClick(rowData: any): any {
            const self = this;
            const selfBase = self as any;
            selfBase.$emit("selected", rowData);
            selfBase.$emit("input", rowData.id);

            if (this.modal) {
                this.modalVisible = false;
            }
        }

        onError(e: any) {
            console.error(e);
        }
    }
</script>
