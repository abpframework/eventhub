module.exports = {
    aliases: {
        "@node_modules": "./node_modules",
        "@libs": "./wwwroot/libs"
    },
    clean: [
        "@libs"
    ],
    mappings: {
        "@node_modules/daterangepicker/*": "@libs/date-range-picker/",
        "@node_modules/owl.carousel/dist/**": "@libs/owl.carousel/",
        "@node_modules/awesomplete/**": "@libs/awesomplete/",
    }
};
