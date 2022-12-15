import AppSettings from "@js/dataModels/settings/appSettings";
import { plainToClassFromExist } from "class-transformer";

export default abstract class Configuration {
    private static readonly _defaultSettingsFile = "settings.json";
    private static readonly _publicConfigurationFolder = "configuration";
    private static _appSettings: Readonly<AppSettings> | null = null;

    public static load(
        environment: string,
        onSettingsLoaded: Function | null = null): Promise<any> {
        this._appSettings = new AppSettings();

        const envSettingsFile = `settings.${(environment || "dev").toLowerCase()}.json`;
        return this.readFromFile(this._defaultSettingsFile)
            .then((_) => {
                this.readFromFile(envSettingsFile)
                    .then((_) => {
                        if (onSettingsLoaded) {
                            onSettingsLoaded.call(this);
                        }
                    })
            });
    }

    public static get appSettings(): Readonly<AppSettings> {
        if (!this._appSettings) {
            throw Error("Please call the 'load' function before accessing this object");
        }
        return this._appSettings;
    };

    private static async readFromFile(jsonFile: string): Promise<any> {
        if (!jsonFile) {
            throw Error("Please specify a valid JSON file to load");
        }

        try {
            // Load from the default settings first
            const response = await fetch(new Request(`/${this._publicConfigurationFolder}/${jsonFile}`));
            const json = await response.json();
            this.setEnvironmentVariables(jsonFile, json);
        }
        catch (e: any) {
            this.logError(jsonFile, `Failed to load ${jsonFile}: ${e.message}`);
        };
    }

    private static setEnvironmentVariables(jsonFile: string, jsonSettings: any): void {
        if (!jsonSettings) {
            throw Error("Please specify a properly formatted JSON file to load");
        }

        try {
            plainToClassFromExist(this._appSettings, jsonSettings.appSettings);
        } catch (e: any) {
            this.logError(jsonFile, `An error occurred while setting configuration variables for ${jsonSettings} ${e.message}`);
        }
    }

    private static logError(context: string, message: string): void {
        console.error(`[Configuration][ERROR][${context}] ${message}`);
    }
}
