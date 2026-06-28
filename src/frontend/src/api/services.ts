import { apiClient } from './client'
import type { Service, PagedResult } from '../types'

export const servicesApi = {
  getAll: async (): Promise<PagedResult<Service>> => {
    const res = await apiClient.get('/services')
    return res.data
  },
  getById: async (id: string): Promise<Service> => {
    const res = await apiClient.get(`/services/${id}`)
    return res.data
  },
  create: async (data: Partial<Service>): Promise<Service> => {
    const res = await apiClient.post('/services', data)
    return res.data
  },
  update: async (id: string, data: Partial<Service>): Promise<Service> => {
    const res = await apiClient.put(`/services/${id}`, data)
    return res.data
  },
  delete: async (id: string): Promise<void> => {
    await apiClient.delete(`/services/${id}`)
  },
}
