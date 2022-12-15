module.exports = {
  preset: '@vue/cli-plugin-unit-jest/presets/no-babel',
  roots: ['<rootDir>'],
  modulePaths: ['<rootDir>'],
  moduleFileExtensions: ['js', 'jsx', 'json', 'vue', 'ts'],
  moduleNameMapper: {
    '@js/(.*)$': '<rootDir>/wwwroot/js/$1',
    '@components/(.*)$': '<rootDir>/wwwroot/components/$1',
  },
  verbose: true,
  "transform": {
    "\\.[jt]sx?$": "babel-jest",
  },
  setupFiles: ['<rootDir>/wwwroot/js/vueConfig/jestVueConfig.ts']
}
