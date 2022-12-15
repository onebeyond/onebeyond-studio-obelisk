"use strict";
const webpack = require('webpack');
const { merge } = require('webpack-merge');
const baseConfig = require('./webpack.config');

module.exports = (env, options) => {

    return merge(baseConfig(env, options), {
        mode: "development",

        //NOTE: see https://webpack.js.org/configuration/devtool/ for more options that may increase your bundling speed
        devtool: "eval-source-map",

        plugins: [
            new webpack.EnvironmentPlugin({
                NODE_ENV: 'development',
                DEBUG: true
            }),
        ],
        devServer: {
            server: {
                type: "https",
                options: {
                    key: "cert.key",
                    cert: "cert.crt",
                    ca: "ca.crt",
                }
            },
        }
    });
};
