interface IStatus {
    id: number;
    name: string;
    outgoingTransitions: IStatusTransition[];
}

export interface IStatusTransition {
    id: number;
    sourceStatusId: number;
    targetStatusId: number;
    targetStatus: IStatus;
    sourceStatus: IStatus;
}

export default IStatus;