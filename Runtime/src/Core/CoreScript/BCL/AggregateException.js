﻿////////////////////////////////////////////////////////////////////////////////
// AggregateException

var ss_AggregateException = function#? DEBUG AggregateException$##(message, innerExceptions) {
	if (typeof(message) !== 'string') {
		innerExceptions = message;
		message = 'One or more errors occurred.';
	}
	innerExceptions = ss.isValue(innerExceptions) ? Array.fromEnumerable(innerExceptions) : null;

	ss_Exception.call(this, message, innerExceptions && innerExceptions.length ? innerExceptions[0] : null);
	this._innerExceptions = innerExceptions;
};
ss_AggregateException.prototype = {
	get_innerExceptions: function#? DEBUG AggregateException$get_innerExceptions##() {
		return this._innerExceptions;
	}
};
Type.registerClass(global, 'ss.AggregateException', ss_AggregateException, ss_Exception);
