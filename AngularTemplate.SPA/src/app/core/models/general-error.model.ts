export interface GeneralErrorModel {
	//succeeded?: boolean;
	code?: string;
	description?: string;
	errors?: GeneralErrorModel[];
}
