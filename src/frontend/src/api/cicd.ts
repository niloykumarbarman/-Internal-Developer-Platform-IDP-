import { apiClient } from './client'
import type { Pipeline } from '../types'

export const cicdApi = {
  getPipelines: async (): Promise<Pipeline[]> => {
    const res = await apiClient.get('/cicd/pipelines')
    return res.data
  },
  triggerPipeline: async (id: string) => {
    const res = await apiClient.post(`/cicd/pipelines/${id}/trigger`)
    return res.data
  },
  getPipelineRuns: async (id: string) => {
    const res = await apiClient.get(`/cicd/pipelines/${id}/runs`)
    return res.data
  },
}
