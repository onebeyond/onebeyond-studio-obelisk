module.exports = {
    "env": {
        "browser": true,
        "es2021": true
    },
    "extends": [
        "eslint:recommended",
    ],
    "plugins": [
        "vue",
        "@typescript-eslint"
    ],
    "parserOptions": {
        "ecmaVersion": "latest",
        "sourceType": "module"
    },
    "rules": {
        "indent": ["error", 4],
        "no-unused-vars": ["error", {
            "varsIgnorePattern": "_",
            "argsIgnorePattern": "_",
            "vars": "local"
        }],
    },
    "overrides": [{
        "files": ["*.vue"],
        "extends": [
            "@vue/typescript",
            "plugin:vue/recommended",
            "eslint:recommended"
        ],
        "parser": "vue-eslint-parser",
        "parserOptions": {
            "ts": "@typescript-eslint/parser",
        },
        "rules": {
            "indent": ["off"],
            "vue/html-indent": ["warn", 4],
            "vue/singleline-html-element-content-newline": ["warn", {
                "ignores": ["h1", "option", "p", "h4", "h5", "label", "a"]
            }],
            "vue/attribute-hyphenation": ["off"],
            "vue/attributes-order": ["off"],
            "vue/max-attributes-per-line": ["error", {
                "singleline": {
                    "max": 4
                },
                "multiline": {
                    "max": 1
                }
            }],
            "vue/mustache-interpolation-spacing": ["warn", "never"],
            "no-unused-vars": ["off"], // JS disabled - TS takes over
            "@typescript-eslint/no-explicit-any": ["off"],
            "vue/no-use-v-if-with-v-for": ["off"],
            "no-extra-boolean-cast": ["off"],
            "vue/no-useless-template-attributes": ["off"],
            "@typescript-eslint/no-unused-vars": ["error", {
                "varsIgnorePattern": "_",
                "argsIgnorePattern": "_",
            }],
        }
    }, {
        "files": ["*.ts"],
        "extends": ["plugin:@typescript-eslint/recommended"],
        "parser": "@typescript-eslint/parser",
        "rules": {
            "no-unused-vars": "off",
            "indent": ["error", 4],
            "@typescript-eslint/no-unused-vars": ["error", {
                "varsIgnorePattern": "_",
                "argsIgnorePattern": "_",
            }],
            "@typescript-eslint/no-explicit-any": ["off"],
            "@typescript-eslint/no-non-null-assertion": ["off"]
        }
    }],

}
