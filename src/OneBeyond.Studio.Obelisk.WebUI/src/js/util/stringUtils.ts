export abstract class StringUtils {

    public static getInitials(input: string | null, charLimit: number = 3): string {
        if (!input) {
            return "";
        }

        const trimmedInput = input.trim();
        // 1. Try a split by space or dash symbol
        let parts = trimmedInput.split(/[ -]/);
        let initials = '';
        for (var i = 0; i < parts.length; i++) {
            initials += parts[i].charAt(0);
        }

        // 2. try a split by Uppercase letter
        if (parts.length < 2) {
            let partsByUppercase = trimmedInput.split(/(?=[A-Z])/);
            // Skip the first initial that is surely already there
            for (var i = 1; i < partsByUppercase.length; i++) {
                initials += partsByUppercase[i].charAt(0);
            }
        }

        // 3. remove numbers from initials whenever possible
        if (initials.length > 2 && initials.search(/[1-9]/) !== -1) {
            initials = initials.replace(/[1-9]+/g, '');
        }

        // 4. crop initials to the maximum number of characters
        initials = initials.substring(0, charLimit).toUpperCase();
        return initials;
    }
}