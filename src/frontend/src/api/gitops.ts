import { apiClient } from './client'

export const gitopsApi = {
  getApplications: async () => {
    const res = await apiClient.get('/gitops/applications')
    return res.data
  },
  syncApplication: async (name: string) => {
    const res = await apiClient.post(`/gitops/applications/${name}/sync`)
    return res.data
  },
  getRepositories: async () => {
    const res = await apiClient.get('/gitops/repositories')
    return res.data
  },
}
