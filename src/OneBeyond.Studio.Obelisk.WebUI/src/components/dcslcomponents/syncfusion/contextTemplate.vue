<template>
    <div class="dropdown context-menu" v-if="isMenuVisible">
        <button
            aria-haspopup="true" 
            aria-expanded="false"
            class="btn btn-secondary btn-outline dropdown-toggle"
            id="dropdownMenuButton" 
            type="button"
        >
            <i class="fas fa-bars" />
        </button>
        <div class="dropdown-menu" aria-labelledby="dropdownMenuButton">
            <div v-for="command in commands" v-if="isCommandVisibleForRow(command.type)">
                <button class="dropdown-item" @click="issueGridAction(command)">
                    <span><i v-if="isIconShown(command)" :class="command.buttonOption.iconCss" /></span>
                    {{command.buttonOption.content}}
                </button>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
    import { Vue, Component } from "vue-property-decorator"
    import { CommandColumnButton } from "@js/grids/syncfusionGrid/types/commandColumnButton";


    @Component({
        name: "ContextMenu",
    })

    export default class ContextMenu extends Vue {

        //The below dictionaries keep track of the menu visibility for each row,
        //seeing as this component is created once for each row

        public rowCommandVisibilityMap: any = {};
        public rowCommandIconVisibilityMap: any = {};
        public isMenuVisible: boolean = false;

        constructor() {
            super();
        }

        public issueGridAction(command: any): void {
            var action = {
                commandColumn: {
                    type: command.type,
                },
                rowData: this.$data.data,

            }
            this.handleCommand(action);
        }

        public mounted(): void {
            this.createRowVisibilityMaps();
        }

        public createRowVisibilityMaps(): void {
            this.commands.forEach(cb => {
                let hiddenIfCondition = cb.hiddenIf;
                if (hiddenIfCondition) {
                    this.rowCommandVisibilityMap[cb.type] = this.$data.data[hiddenIfCondition.fieldName] != hiddenIfCondition.value;
                }
                else {
                    this.rowCommandVisibilityMap[cb.type] = true;
                }
            })

            this.commands.forEach(cb => {
                let iconHiddenCondition = cb.hiddenIconIf;
                if (iconHiddenCondition) {
                    this.rowCommandIconVisibilityMap[cb.type] = this.$data.data[iconHiddenCondition.fieldName] != iconHiddenCondition.value;
                }
                else {
                    this.rowCommandIconVisibilityMap[cb.type] = true;
                }
            });

            this.updateMenuVisibility();
        }

        //extended by grid class
        public handleCommand(data: any): void { }

        public isCommandVisibleForRow(type: string): boolean {
            return this.rowCommandVisibilityMap[type];
        }

        public updateMenuVisibility(): void {
            this.isMenuVisible = Object.values(this.rowCommandVisibilityMap).filter(x => x).length > 0;
        }

        public isIconShown(command: CommandColumnButton): boolean {
            return command.buttonOption.iconCss && this.rowCommandIconVisibilityMap[command.type];
        }

        public get commands(): Array<CommandColumnButton> { return new Array<CommandColumnButton>() }
    }
</script>