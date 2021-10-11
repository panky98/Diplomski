export interface EventCreated
{
    creatorId:number;
    code:string;
    name:string;
    dateTimeOfEvent:Date;
    interestIds:number[];
    base64:string;
}
