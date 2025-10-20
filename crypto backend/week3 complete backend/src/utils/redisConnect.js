const redis = require('redis');

let redisClient = redis.createClient();

redisClient.on('error', (err) => {
  console.log('Error connecting to redis :' + err);
});

redisClient.on('ready', () => {
  console.log('Connected to redis');
});

module.exports = redisClient;
