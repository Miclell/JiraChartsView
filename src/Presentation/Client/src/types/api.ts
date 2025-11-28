// Types for API responses

export interface JiraIssue {
  key: string;
  fields: {
    created: string;
    updated: string;
    resolutiondate: string | null;
    status: {
      name: string;
      statusCategory?: {
        name: string;
      };
    };
    priority: {
      name: string;
      id: string;
    };
    assignee: {
      displayName: string;
      accountId: string;
      emailAddress?: string;
      active: boolean;
    } | null;
    reporter: {
      displayName: string;
      accountId: string;
      emailAddress?: string;
      active: boolean;
    } | null;
  };
}

export interface JiraSearchResponse {
  issues: JiraIssue[];
}

export interface DailyFlowItemDto {
  date: string;
  createdCount: number;
  resolvedCount: number;
}

export interface DailyTaskFlowDto {
  projectKey: string;
  dailyFlow: DailyFlowItemDto[];
}

export interface PriorityStatsDto {
  priority: string;
  count: number;
  percentage: number;
}

export interface PriorityDistributionDto {
  projectKey: string;
  totalIssues: number;
  distribution: PriorityStatsDto[];
}

export interface StatusChangeDto {
  fromStatus: string;
  toStatus: string;
  changeDate: string;
}

export interface IssueStatusTimeDto {
  issueKey: string;
  created: string | null;
  resolutionDate: string | null;
  currentStatus: string | null;
  statusChanges: StatusChangeDto[];
}

export interface StatusTimeDistributionDto {
  projectKey: string;
  issues: IssueStatusTimeDto[];
}

export interface UserStatsDto {
  userName: string;
  totalCount: number;
  reporterCount: number;
  assigneeCount: number;
}

export interface TopUsersDto {
  projectKey: string;
  topUsers: UserStatsDto[];
}

export interface WorklogHistogramItemDto {
  timeRange: string;
  taskCount: number;
}

export interface WorklogHistogramDto {
  projectKey: string;
  histogram: WorklogHistogramItemDto[];
}
