/**
 * This object should store all application settings
 * defined in the settings.json or settings.<ENV>.json
 * Please keep it in sync with JSON files
 */
export default class AppSettings {
    apiUrl: string = "";
    allowApiUrlOverrideFromDevTools: boolean = false;
    sessionTimeoutInMinutes: number = 60;
}