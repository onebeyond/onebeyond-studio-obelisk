import { ShortDate, LongDateTime, MonthYearDate } from './constants';
import { format } from 'date-fns';


function showYesNo(value: boolean) {
    return value ? 'Yes' : 'No';
}

function shortDate(value: string | number | Date) {
    if (!value) {
        return "";
    }
    else {
        const date = new Date(value);
        return format(date, ShortDate);
    }
}

function longDateTime(value: string | number | Date) {
    if (!value) {
        return "";
    }
    else {
        const date = new Date(value);
        return format(date, LongDateTime);
    }
}

function monthYearDate(value: string | number | Date) {
    if (!value) {
        return "";
    }
    else {
        const date = new Date(value);
        return format(date, MonthYearDate);
    }
}

function sizeInKb(value) {
    if (!value) return value;
    return (+value / 1024).toFixed(2);
}

function currency(value, symbol, decimals, options) {
    let thousandsSeparator, symbolOnLeft, spaceBetweenAmountAndSymbol;
    const digitsRE = /(\d{3})(?=\d)/g;
    options = options || {};
    value = parseFloat(value);
    if (!isFinite(value) || (!value && value !== 0)) return '';
    symbol = symbol != null ? symbol : '$';
    decimals = decimals != null ? decimals : 2;
    thousandsSeparator = options.thousandsSeparator != null ? options.thousandsSeparator : ',';
    symbolOnLeft = options.symbolOnLeft != null ? options.symbolOnLeft : true;
    spaceBetweenAmountAndSymbol = options.spaceBetweenAmountAndSymbol != null ? options.spaceBetweenAmountAndSymbol : false;
    let stringified = Math.abs(value).toFixed(decimals);
    stringified = options.decimalSeparator
        ? stringified.replace('.', options.decimalSeparator)
        : stringified;
    const _int = decimals
        ? stringified.slice(0, -1 - decimals)
        : stringified;
    const i = _int.length % 3;
    const head = i > 0
        ? (_int.slice(0, i) + (_int.length > 3 ? thousandsSeparator : ''))
        : '';
    const _float = decimals
        ? stringified.slice(-1 - decimals)
        : '';
    symbol = spaceBetweenAmountAndSymbol
        ? (symbolOnLeft ? symbol + ' ' : ' ' + symbol)
        : symbol;
    symbol = symbolOnLeft
        ? symbol + head +
        _int.slice(i).replace(digitsRE, '$1' + thousandsSeparator) + _float
        : head +
        _int.slice(i).replace(digitsRE, '$1' + thousandsSeparator) + _float + symbol;
    const sign = value < 0 ? '-' : '';
    return sign + symbol;
}

export { showYesNo, shortDate, longDateTime, monthYearDate, currency, sizeInKb }
