!function(n) {
    "use strict";
    function t(n, t) {
        return n + t;
    }
    function o(n, t) {
        return n - t;
    }
    var r = Math.random() > .5 ? {
        fn: t
    } : {
        fn: o
    };
    console.log(r.fn(1, 2));
}();