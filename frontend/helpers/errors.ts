export interface ValidationError {
    response: {
        data: {
            instance: string;
            status: number;
            title: string;
        },
        status: number;
        statusText: string;
    }
}