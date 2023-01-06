<template>
    <ejs-grid
        :id="entityGrid.id"
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
        :selectionSettings="entityGrid.selectionSettings"
        :searchSettings="entityGrid.searchSettings"
        :sortSettings="entityGrid.sortSettings"
        :toolbar="entityGrid.toolbarOptions"
        @actionBegin="(args) => entityGrid.onActionBegin(args)"
        @commandClick="(args) => entityGrid.onCommandClick(args)"
        @checkBoxChange="(args) => entityGrid.onCheckboxChanged(args)"
        @beginEdit="(args) => entityGrid.onBeginEdit(args)"              
        @rowDataBound="(args) => entityGrid.onRowDataBound(args)"
        @rowDrop="(args) => entityGrid.onRowDropped(args)"
    >
        <e-columns>
            <slot name="columns">
                <!--CheckBox Selection Column-->
                <e-column
                    v-if="entityGrid.isSelectionColumnVisible"
                    :allowFiltering="false"
                    :allowSorting="false"
                    field="Selection"
                    :freeze="entityGrid.selectionColumnFreezePosition"
                    :hideAtMedia="entityGrid.mobileColumnVisible ? entityGrid.columnBreakpointCss : ''"
                    type="checkbox"
                    :visible="entityGrid.isSelectionColumnVisible"
                    width="100"
                />

                <e-column
                    v-for="(column, index) in entityGrid.columns"
                    :allowEditing="column.allowEditing"
                    :allowFiltering="column.allowFiltering"
                    :allowSorting="column.allowSorting"
                    :edit="column.inlineEditSettings"
                    :editType="column.inlineEditType"
                    :displayAsCheckBox="column.displayAsCheckBox"
                    :field="column.fieldName"
                    :format="column.format"
                    :filter="column.filterSettings"
                    :freeze="column.freezePosition"
                    :headerText="column.headerText"
                    :hideAtMedia="entityGrid.columnBreakpointCss"
                    :isPrimaryKey="column.isPrimaryKey"
                    :key="index"
                    :template="column.getColumnTemplate"
                    :type="column.fieldType"
                    :valueAccessor="column.valueAccessor"
                    :validationRules="column.validationRules"
                    :width="column.width ? column.width : 150"
                    :visible="column.isVisible"
                    :defaultValue="column.defaultValue"
                    :multiline="column.multiline"
                    :minDate="column.minDate"
                    :maxDate="column.maxDate"
                />

                <!--Actions Column-->
                <e-column
                    :allowSorting="false"
                    :allowFiltering="false"
                    :commands="entityGrid.commands"
                    field="Actions"
                    headerText=""
                    :hideAtMedia="entityGrid.columnBreakpointCss"
                    textAlign="Right"
                    :visible="entityGrid.isCommandColumnVisible"
                    :width="entityGrid.actionsWidth ? entityGrid.actionsWidth : 200"
                    freeze="entityGrid.commandColumnFreezePosition"
                />

                <!--Mobile Column-->
                <e-column
                    :allowSorting="false"
                    :allowFiltering="false"
                    :allowEditing="false"
                    field="Mobile"
                    headerText=""
                    :hideAtMedia="entityGrid.mobileViewBreakpointCss"
                    :template="entityGrid.getMobileTemplate"
                    :visible="entityGrid.isMobileColumnVisible"
                />
            </slot>
        </e-columns>
    </ejs-grid>
</template>

<script lang="ts">
    import { Vue, Component, Prop } from "vue-property-decorator"

    import {
        GridPlugin,
        Grid,
        Page,
        Sort,
        Filter,
        CommandColumn,
        DetailRow,
        ExcelExport,
        Search,
        Toolbar,
        Edit,
        Freeze,
        RowDD
    } from '@syncfusion/ej2-vue-grids';

    import { SyncfusionEntityGrid } from "@js/grids/syncfusionGrid/syncfusionGrid";
    import { MultiSelect, CheckBoxSelection } from '@syncfusion/ej2-dropdowns';
    import { MultiSelectPlugin } from "@syncfusion/ej2-vue-dropdowns";

    Vue.use(GridPlugin);
    MultiSelect.Inject(CheckBoxSelection);
    Vue.use(MultiSelectPlugin);
    Grid.Inject(Freeze);

    @Component({
        name: "DCSLSyncfusionGrid",
        provide: () => { return { grid: [RowDD, Search, Toolbar, Edit, ExcelExport, Page, Sort, Filter, CommandColumn, DetailRow, Freeze] }; }
    })

    export default class DCSLSyncfusionGrid extends Vue {
        @Prop({ type: SyncfusionEntityGrid }) entityGrid!: any;
    }
</script>

<style>
    @import '@styles/_components/_syncfusion.scss';
</style>