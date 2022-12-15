"use strict";

module.exports = (env, options) => {
    const webpack = require("webpack");
    const { merge } = require('webpack-merge');
    const baseConfig = require("./webpack.config");
    const TerserPlugin = require("terser-webpack-plugin");
    const CssMinimizerWebpackPlugin = require("css-minimizer-webpack-plugin");

    return merge(baseConfig(env, options), {
        mode: "production",
        //Stop creating source maps which in production we don't need
        devtool: false,
        plugins: [
            new CssMinimizerWebpackPlugin({}),
            new webpack.EnvironmentPlugin({
                NODE_ENV: "production",
                DEBUG: false
            })
        ],
        optimization: {
            minimize: true,
            minimizer: [new TerserPlugin({ parallel: true })]
        },
        resolve: {
            alias: {
                vue: 'vue/dist/vue.min.js'
            }
        }
    });
};