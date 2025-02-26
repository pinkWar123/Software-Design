// globalMessage.ts
import React from 'react';
export interface GlobalMessageApi {
    info: (content: React.ReactNode, duration?: number, onClose?: () => void) => void;
    success: (content: React.ReactNode, duration?: number, onClose?: () => void) => void;
    error: (content: React.ReactNode, duration?: number, onClose?: () => void) => void;
    warning: (content: React.ReactNode, duration?: number, onClose?: () => void) => void;
    warn: (content: React.ReactNode, duration?: number, onClose?: () => void) => void;
    loading: (content: React.ReactNode, duration?: number, onClose?: () => void) => void;
  }

let globalMessageApi: GlobalMessageApi | null = null;

export const setGlobalMessageApi = (api: GlobalMessageApi) => {
  globalMessageApi = api;
};

export const getGlobalMessageApi = (): GlobalMessageApi => {
  if (!globalMessageApi) {
    throw new Error('Global message API is not set.');
  }
  return globalMessageApi;
};
