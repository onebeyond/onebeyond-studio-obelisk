const webpack = require('webpack');
const path = require('path');
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const { VueLoaderPlugin } = require('vue-loader');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const CopyPlugin = require("copy-webpack-plugin");

module.exports = (env, options) => {


    const webpackConfig = {
        entry: {
            indexApp: './src/js/indexApp.ts',
            adminApp: './src/js/adminApp.ts',
            authApp: './src/js/authApp.ts',
            vendor: [
                "jquery",
                "bootstrap"
            ],
            mainCss: './css/site.scss'
        },

        output: {
            publicPath: "/",
            path: path.join(__dirname, '/dist/'),
            filename: '[name].[contenthash].bundle.js',
            clean: true,
            devtoolModuleFilenameTemplate: info => {
                var $filename = "sources://" + info.resourcePath;
                if (info.resourcePath.match(/\.vue$/) && !info.query.match(/type=script/)) {
                    $filename = "webpack-generated:///" + info.resourcePath + "?" + info.hash;
                }
                return $filename;
            },
            devtoolFallbackModuleFilenameTemplate: "webpack:///[resource-path]?[hash]"
        },

        target: ['web', 'es5'],

        plugins: [
            new CopyPlugin({
                patterns: [
                    { from: "assets", to: "assets" }
                ],
            }),
            new CopyPlugin({
                patterns: [
                    { from: "public/configuration", to: "configuration" }
                ],
            }),
            new CopyPlugin({
                patterns: [
                    { from: "staticwebapp.config.json", to: "staticwebapp.config.json" }
                ],
            }),
            new HtmlWebpackPlugin({
                filename: 'index.html',
                template: './public/index.html',
                excludeChunks: ['adminApp', 'authApp']
            }),
            new HtmlWebpackPlugin({
                filename: 'auth.html',
                template: './public/auth.html',
                excludeChunks: ['indexApp', 'adminApp']
            }),
            new HtmlWebpackPlugin({
                filename: 'admin.html',
                template: './public/admin.html',
                excludeChunks: ['indexApp', 'authApp']
            }),
            new VueLoaderPlugin(),
            new webpack.ProvidePlugin({
                process: 'process/browser',
                Popper: ['popper.js', 'default'],
                vueRouter: 'vue-router',
                vuex: 'vuex'
            }),
            new MiniCssExtractPlugin({
                filename: "[name].[contenthash].bundle.css",
                ignoreOrder: true
            }),
            new webpack.EnvironmentPlugin({
                BUILD_NUMBER: env.BUILD_NUMBER || "N/A",
                BUILD_DATE: env.BUILD_DATE || new Date().toUTCString()
            })
        ],

        optimization: {
            splitChunks: {
                chunks: "async",
                minSize: 20000,
                minChunks: 1,
                maxAsyncRequests: 30,
                maxInitialRequests: 30,
                cacheGroups: {
                    defaultVendors: {
                        test: "/[\\/]node_modules[\\/]|vendor[\\/]analytics_provider|vendor[\\/]other_lib/",
                        priority: -10,
                        reuseExistingChunk: true,
                        filename: "[name].[contenthash].bundle.js"
                    },
                    default: {
                        minChunks: 2,
                        priority: -20,
                        reuseExistingChunk: true
                    }
                }
            }
        },

        resolve: {
            alias: {
                vue: 'vue/dist/vue.js',
                '@assets': path.resolve(__dirname, 'assets/'),
                '@js': path.resolve(__dirname, 'src/js/'),
                '@components': path.resolve(__dirname, 'src/components/'),
                '@styles': path.resolve(__dirname, 'css/')
            },
            extensions: ['.ts', '.tsx', '.js', '.jsx', '.vue', '.json'],
            fallback: {
                "path": require.resolve("path-browserify")
            },
        },

        module: {
            rules: [
                //vue
                {
                    test: /\.vue$/,
                    loader: 'vue-loader',
                },

                // ts
                {
                    test: /\.tsx?$/,
                    use: [
                        'thread-loader',
                        'babel-loader',
                        {
                            loader: 'ts-loader',
                            options: {
                                transpileOnly: true,
                                appendTsSuffixTo: [/\.vue$/],
                                happyPackMode: true,
                            },
                        },
                    ],
                },

                //babel
                {
                    test: /\.m?jsx?$/,
                    exclude: (file) => {
                        // always transpile js in vue files
                        if (/\.vue\.jsx?$/.test(file)) {
                            return false
                        }
                        // Don't transpile node_modules
                        return /node_modules/.test(file)
                    },
                    use: ['thread-loader', 'babel-loader'],
                },

                //css
                {
                    test: /\.(sa|sc|c)ss$/,
                    use: [MiniCssExtractPlugin.loader,
                    { loader: 'css-loader', options: { sourceMap: false } },
                    { loader: 'sass-loader', options: { sourceMap: false } }
                    ]
                },

                //NOTE: see https://webpack.js.org/guides/asset-modules/ and https://survivejs.com/webpack/loading/images/
                //asset type will inline files up to 8kb by default
                //otherwise they will be copied to dist and downloaded via query
                {
                    test: /\.(svg|png|jpg|gif)$/, // If you add image with different extension add the extension here
                    type: 'asset',
                    generator: { filename: './images/[name].[ext]' }
                },

                // media
                {
                    test: /\.(mp4|webm|ogg|mp3|wav|flac|aac)(\?.*)?$/,
                    type: 'asset',
                    generator: { filename: 'media/[contenthash:8][ext][query]' },
                },

                //fonts
                {
                    test: /\.(eot|ttf|woff|woff2)$/,
                    type: 'asset',
                    generator: { filename: './fonts/[name][ext]' }
                },
            ]
        },

        devServer: {
            static: {
                directory: './dist',
            },
            server: 'https',
            port: 8080,
            devMiddleware: {
                publicPath: '/dist/',
                writeToDisk: true,
            },

            historyApiFallback: {
                rewrites: [
                    { from: /\/auth/, to: '/auth.html' },
                    { from: /\/admin/, to: '/admin.html' },
                    { from: /\//, to: '/index.html' }
                ]
            },

            open: ['/']
        }
    }

    return webpackConfig;
}