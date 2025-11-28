import axios from 'axios';
import type {
  DailyTaskFlowDto,
  JiraSearchResponse,
  PriorityDistributionDto,
  StatusTimeDistributionDto,
  TopUsersDto,
  WorklogHistogramDto,
} from '../types/api';

const API_PATH = '/api/jira-analytics';
const envBaseUrl = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5085';

const API_BASE_URL = envBaseUrl
  ? envBaseUrl.endsWith(API_PATH)
    ? envBaseUrl
    : `${envBaseUrl.replace(/\/$/, '')}${API_PATH}`
  : API_PATH;

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

export const jiraApi = {
  getOpenTimeHistogram: async (projectKey: string): Promise<JiraSearchResponse> => {
    const response = await apiClient.get<JiraSearchResponse>(
      `/${projectKey}/open-time-histogram`
    );
    return response.data;
  },

  getStatusTimeDistribution: async (
    projectKey: string
  ): Promise<StatusTimeDistributionDto> => {
    const response = await apiClient.get<StatusTimeDistributionDto>(
      `/${projectKey}/status-time-distribution`
    );
    return response.data;
  },

  getDailyTaskFlow: async (projectKey: string): Promise<DailyTaskFlowDto> => {
    const response = await apiClient.get<DailyTaskFlowDto>(
      `/${projectKey}/daily-task-flow`
    );
    return response.data;
  },

  getTopUsers: async (projectKey: string): Promise<TopUsersDto> => {
    const response = await apiClient.get<TopUsersDto>(
      `/${projectKey}/top-users`
    );
    return response.data;
  },
    
  getPriorityDistribution: async (
    projectKey: string
  ): Promise<PriorityDistributionDto> => {
    const response = await apiClient.get<PriorityDistributionDto>(
      `/${projectKey}/priority-distribution`
    );
    return response.data;
  },

  getWorklogHistogram: async (
    projectKey: string
  ): Promise<WorklogHistogramDto> => {
    const response = await apiClient.get<WorklogHistogramDto>(
      `/${projectKey}/worklog-histogram`
    );
    return response.data;
  },
};

