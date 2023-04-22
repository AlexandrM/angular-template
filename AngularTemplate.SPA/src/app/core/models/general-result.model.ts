import { GeneralErrorModel } from './general-error.model';

export interface GeneralResultModel {
	success: boolean;
	error?: GeneralErrorModel;
	data?: any;
}
