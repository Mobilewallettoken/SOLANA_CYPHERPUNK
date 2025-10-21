const jwt = require('jsonwebtoken');

const { User } = require('../models/users/user.js');

async function auth(req, res, next) {
  try {
    let token = req.body.refreshToken;
    if (!token) return res.status(400).send('Token not Provided');
    const decoded = jwt.verify(token, process.env.REFRESH_TOKEN_PRIVATE_KEY);
    req.user = decoded;
    const user = await User.findById(req.user._id).select('-password');
    if (!user) return res.status(400).send('Invalid Token: User do not exist');
    req.user = user;
  } catch (error) {
    return res.status(401).send('Invalid Token');
  }
  next();
}

module.exports = auth;
