export interface User {
  id: string
  email: string
  firstName: string
  lastName: string
  role: 'Admin' | 'PlatformEngineer' | 'Developer'
  teamId?: string
  teamName?: string
}

export interface AuthState {
  user: User | null
  token: string | null
  isAuthenticated: boolean
  isLoading: boolean
}

export interface Service {
  id: string
  name: string
  slug: string
  description: string
  type: string
  status: string
  ownerTeamId: string
  ownerTeamName: string
  repositoryUrl?: string
  apiDocumentationUrl?: string
  tags: string[]
  createdAt: string
}

export interface Pipeline {
  id: string
  name: string
  serviceId: string
  serviceName: string
  status: string
  branch: string
  triggeredBy: string
  startedAt: string
  completedAt?: string
  environment: string
}

export interface KubernetesDeployment {
  id: string
  name: string
  namespace: string
  image: string
  replicas: number
  status: string
  environment: string
  createdAt: string
}

export interface Incident {
  id: string
  title: string
  description: string
  severity: string
  status: string
  serviceId: string
  serviceName: string
  createdAt: string
  resolvedAt?: string
}

export interface Repository {
  id: string
  name: string
  fullName: string
  url: string
  isPrivate: boolean
  defaultBranch: string
  createdAt: string
}

export interface Team {
  id: string
  name: string
  slug: string
  description?: string
  memberCount: number
}

export interface DashboardStats {
  totalServices: number
  totalPipelines: number
  activeDeployments: number
  openIncidents: number
  successRate: number
  totalTeams: number
}
export interface PagedResult<T> {
  items: T[]
  totalCount: number
  page: number
  pageSize: number
}
