var HTMLWebpackPlugin = require('html-webpack-plugin');
var HTMLWebpackPluginConfig = new HTMLWebpackPlugin({
	template: __dirname + '/app/index.html',
	filename: 'index.html',
	inject: 'body'
})
var ExtractTextPlugin = require("extract-text-webpack-plugin");

module.exports = {
	entry: __dirname + '/app/index.js',
	module: {
		loaders: [
			{
				test: /\.js$/,
				exclude: /node_modules/,
				loader: 'babel-loader'
			},
			{ 
				test: /\.css$/, 
				loader: ExtractTextPlugin.extract("style-loader", "css-loader")
			},
			{ 
				test: /\.(eot|woff|woff2|ttf|svg|png|jpg|gif)([\?]?.*)$/,
				loader: 'url-loader' 
			}
		]
	},
	resolve: {
            modules: ['node_modules'],
            extensions: [
                '',
                '.js',
                '.jsx',
                '.react.js'
            ]
    },
	output: {
		filename: 'bundle.js',
		path: __dirname + '/build'
	},
	plugins: [
		HTMLWebpackPluginConfig,
		new ExtractTextPlugin("styles.css")
	]
};