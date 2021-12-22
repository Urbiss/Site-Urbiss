export class Order {
  UserSurveyId: number;
  surveyid: number;
  voucherCode: string;

  public constructor(
    _UserSurveyId: number, 
	_surveyid: number,
	_voucherCode: string,
  ) {
    this.UserSurveyId = _UserSurveyId;
    this.surveyid = _surveyid;
	this.voucherCode = _voucherCode;
  }
}
  