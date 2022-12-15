<template>
    <ejs-grid :id="entityGrid.id"
              ref="entityGrid"
              :allowSorting="true"
              :allowExcelExport="true"
              :allowFiltering="true"
              :allowRowDragAndDrop="entityGrid.isRowDragAndDropEnabled"
              :allowTextWrap="true"
              :allowPaging="true"
              :dataSourceChanged="(args) => entityGrid.onDataSourceChanged(args)"
              :dataStateChange="(args) => entityGrid.onDataStateChanged(args)"
              :detailTemplate="entityGrid.getDetailTemplate"
              :editSettings="entityGrid.editSettings"
              :enableResponsiveRow="true"
              :enablePersistence="entityGrid.isPersistenceSupported"
              :excelQueryCellInfo="(args) => entityGrid.formatExcelExportData(args)"
              :filterSettings="entityGrid.filterSettings"
              :isResponsive="true"
              :pageSettings="entityGrid.pageSettings"
              :searchSettings="entityGrid.searchSettings"
              :selectionSettings="entityGrid.selectionSettings"
              :sortSettings="entityGrid.sortSettings"
              :toolbar="entityGrid.toolbarOptions.length > 0 ? entityGrid.toolbarOptions : ''"
              @actionBegin="(args) => entityGrid.onActionBegin(args)"
              @commandClick="(state) => entityGrid.onCommandClick(state)"
              @checkBoxChange="(args) => entityGrid.onCheckboxChanged(args)"
              @beginEdit="(args) => entityGrid.onBeginEdit(args)"
              @rowDataBound="(args) => entityGrid.onRowDataBound(args)"
              @rowDrop="(args) => entityGrid.onRowDropped(args)">
        <e-columns>
            <slot name="columns">

                <!--CheckBox Selection Column-->
                <e-column :allowFiltering="false"
                          :allowSorting="false"
                          field="Selection"
                          :freeze="entityGrid.selectionColumnFreezePosition"
                          :hideAtMedia="entityGrid.mobileColumnVisible ? entityGrid.columnBreakpointCss : ''"
                          type='checkbox'
                          :visible="entityGrid.isSelectionColumnVisible"
                          width="100">
                </e-column>

                <!--ContextMenu Column-->
                <e-column :customAttributes="customAttributes"
                          headerText="Menu"
                          field="Menu"
                          :allowEditing="false"
                          :allowSorting="false"
                          :allowFiltering="false"
                          :freeze="entityGrid.contextMenuColumnFreezePosition"
                          :hideAtMedia="entityGrid.mobileColumnVisible ? entityGrid.columnBreakpointCss : ''"
                          :template="getContextMenuTemplate"
                          :visible="entityGrid.isContextMenuColumnVisible"
                          width="90">
                </e-column>

                <e-column v-for="(column, index) in entityGrid.columns"
                          :allowEditing="column.allowEditing"
                          :allowFiltering="column.allowFiltering"
                          :allowSorting="column.allowSorting"
                          :customAttributes="column.customAttributes"
                          :edit="column.inlineEditSettings"
                          :editType="column.inlineEditType"
                          :displayAsCheckBox="column.displayAsCheckBox"
                          :field="column.fieldName"
                          :filter="column.filterSettings"
                          :format="column.format"
                          :freeze="column.freezePosition"
                          :headerText="column.headerText"
                          :hideAtMedia="entityGrid.mobileColumnVisible ? entityGrid.columnBreakpointCss : ''"
                          :isPrimaryKey="column.isPrimaryKey"
                          :key="index"
                          :template="column.getColumnTemplate"
                          :type="column.fieldType"
                          :valueAccessor="column.valueAccessor"
                          :validationRules="column.validationRules"
                          :visible="column.isVisible"
                          :width="column.width ? column.width : 150"
                          :defaultValue="column.defaultValue"
                          :multiline="column.multiline"
                          :minDate="column.minDate"
                          :maxDate="column.maxDate">
                </e-column>

                <!--Mobile Column-->
                <e-column :allowSorting="false"
                          :allowFiltering="false"
                          :allowEditing="false"
                          field="Mobile"
                          headerText=""
                          :hideAtMedia="entityGrid.mobileViewBreakpointCss"
                          :template="entityGrid.getMobileTemplate"
                          :visible="entityGrid.mobileColumnVisible">
                </e-column>
            </slot>
        </e-columns>
    </ejs-grid>
</template>

<script lang="ts">
    import { Vue, Component, Prop } from "vue-property-decorator"

    import {
        GridPlugin,
        Page,
        Sort,
        Filter,
        DetailRow,
        ExcelExport,
        Search,
        Toolbar,
        Edit,
        RowDD,
        Freeze,
        Grid
    } from '@syncfusion/ej2-vue-grids';

    import { SyncfusionEntityGrid } from "@js/grids/syncfusionGrid/syncfusionGrid";
    import { MultiSelect, CheckBoxSelection } from '@syncfusion/ej2-dropdowns';
    import { MultiSelectPlugin } from "@syncfusion/ej2-vue-dropdowns";
    import ContextMenu from "./contextTemplate.vue";

    Vue.use(GridPlugin);
    MultiSelect.Inject(CheckBoxSelection);
    Vue.use(MultiSelectPlugin);
    Grid.Inject(Freeze);

    @Component({
        name: "DCSLSyncfusionContextGrid",
        provide: () => { return { grid: [RowDD, Search, Toolbar, Edit, ExcelExport, Page, Sort, Filter, DetailRow, Freeze] }; } 
    })

    export default class DCSLSyncfusionContextGrid extends Vue {
        @Prop({ type: SyncfusionEntityGrid }) entityGrid!: SyncfusionEntityGrid;
        public customAttributes: any;

        constructor() {
            super();
            this.customAttributes = { class: 'column-contextmenu' };
        }


        public getContextMenuTemplate(): any {
            return {
                template: Vue.component('context-menu',
                    {
                        extends: ContextMenu,
                        methods: {
                            handleCommand: (args) => this.entityGrid.onCommandClick(args) //required to correctly wire up context menu actions to grid
                        },
                        computed: {
                            commands: () => this.entityGrid.commands
                        }
                    }),
            }
        }
    }
</script>

<style>
    @import '@styles/_components/_syncfusion.scss';
</style>