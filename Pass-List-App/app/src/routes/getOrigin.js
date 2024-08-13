const db = require('../persistence');

module.exports = async (req, res) => {
    const origin = await db.getDataOrigin();
    res.send(origin);
};
