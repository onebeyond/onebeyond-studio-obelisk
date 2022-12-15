import { ShortDate } from '@js/util/constants';
import { format } from 'date-fns';

const PHONEREG = /^\(?0( *\d\)?){9,10}$/;

const PhoneValidator = {
    getMessage(field, args) {
        return 'Phone number must start with "0" and be 10 or 11 digits long';
    },
    validate(value, args) {
        return PHONEREG.test(value);
    }
};

const COMPLEXPASS = /^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#\$%\^&\*])(?=.{8,})/;

const ComplexPasswordValidator = {
    getMessage(field, args) {
        return 'The password must contain at least: 1 uppercase letter, 1 lowercase letter, 1 number, and one special character (!@#\$%\^&\*)';
    },
    validate(value, args) {
        return COMPLEXPASS.test(value);
    }
};

const DateRangeValidator = {
    getMessage(field, args) {
        if (args[1] != "min" && args[1] != "max") {
            var fromDate = new Date(args[0]);
            var toDate = new Date(args[1]);

            return `The date must be between ${format(fromDate, ShortDate)} and ${format(toDate, ShortDate)}`;
        } else {
            if (args[1] == "min") {
                var checkDate = new Date(args[0]);

                return `The date cannot be earlier than ${format(checkDate, ShortDate)}`;
            } else {
                var checkDate = new Date(args[0]);

                return `The date cannot be later than ${format(checkDate, ShortDate)}`;
            }
        }
    },
    validate(value, args) {
        var dateVal = new Date(value);

        if (args[1] != "min" && args[1] != "max") {
            var fromDate = new Date(args[0]);
            var toDate = new Date(args[1]);

            return dateVal >= fromDate && dateVal <= toDate;
        } else {
            if (args[1] == "min") {
                var dateCheck = new Date(args[0]);
                return dateVal >= dateCheck;
            } else {
                var dateCheck = new Date(args[0]);
                return dateVal <= dateCheck;
            }
        }
    }
};

export { PhoneValidator, ComplexPasswordValidator, DateRangeValidator };
