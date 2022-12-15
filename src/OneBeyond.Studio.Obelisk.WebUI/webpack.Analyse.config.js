"use strict";
const { merge } = require('webpack-merge');
const developmentConfig = require('./webpack.Debug.config');
const productionConfig = require('./webpack.Release.config')
const BundleAnalyzerPlugin = require('webpack-bundle-analyzer').BundleAnalyzerPlugin;

const analyzeConfig = {
    plugins: [
        new BundleAnalyzerPlugin()
    ]
};

module.exports = (env, options) => {
    switch (env.NODE_ENV) {
        case 'development':
            return merge(developmentConfig(env, options), analyzeConfig);
        case 'production':
            return merge(productionConfig(env, options), analyzeConfig);
        default:
            throw new Error('No matching configuration was found!');
    }
}