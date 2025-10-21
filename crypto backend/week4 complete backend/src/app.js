var express = require('express');
var path = require('path');
var cookieParser = require('cookie-parser');
var logger = require('morgan');
var { config } = require('dotenv');
config();
var swaggerUi = require('swagger-ui-express');

var dbConnect = require('./utils/dbConnect.js');
var swaggerDocs = require('./utils/swagger.js');

var quoteRouter = require('./routes/quote.js');
var listRouter = require('./routes/list.js');
var settingRouter = require('./routes/settings.js');
// const redisClient = require('./utils/redisConnect.js');

dbConnect();
// redisClient.connect();

var app = express();

app.use(logger('dev'));
app.use(express.json());
app.use(express.urlencoded({ extended: false }));
app.use(cookieParser());
app.use(express.static(path.join(__dirname, 'public')));
// Swagger route
app.use('/api-docs', swaggerUi.serve, swaggerUi.setup(swaggerDocs));
app.get('/api-json', (_req, res) => {
    res.setHeader('Content-Type', 'application/json');
    res.send(swaggerDocs);
});

// routes
app.use('/quote', quoteRouter);
app.use('/list', listRouter);
app.use('/settings', settingRouter);

// app.use('/list', currencyConversionRouter);
app.get('/*', function (_req, res) {
    res.sendFile(path.join(__dirname, '../public/index.html'), function (err) {
        if (err) {
            console.log(err);
            res.status(500).send(err);
        }
    });
});
module.exports = app;