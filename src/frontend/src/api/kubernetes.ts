import { apiClient } from './client'
import type { KubernetesDeployment } from '../types'

export const kubernetesApi = {
  getDeployments: async (): Promise<KubernetesDeployment[]> => {
    const res = await apiClient.get('/kubernetes/deployments')
    return res.data
  },
  getNamespaces: async () => {
    const res = await apiClient.get('/kubernetes/namespaces')
    return res.data
  },
  getPods: async (namespace: string) => {
    const res = await apiClient.get(`/kubernetes/namespaces/${namespace}/pods`)
    return res.data
  },
  scaleDeployment: async (name: string, namespace: string, replicas: number) => {
    const res = await apiClient.post(`/kubernetes/deployments/${name}/scale`, { namespace, replicas })
    return res.data
  },
}
