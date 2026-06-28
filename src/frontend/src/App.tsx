import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { useAuthStore } from './store/authStore'
import AppLayout from './components/layout/AppLayout'
import LoginPage from './pages/auth/LoginPage'
import RegisterPage from './pages/auth/RegisterPage'
import DashboardPage from './pages/dashboard/DashboardPage'
import CatalogPage from './pages/catalog/CatalogPage'
import CicdPage from './pages/cicd/CicdPage'
import KubernetesPage from './pages/kubernetes/KubernetesPage'
import ObservabilityPage from './pages/observability/ObservabilityPage'
import GitOpsPage from './pages/gitops/GitOpsPage'
import VaultPage from './pages/vault/VaultPage'
import IncidentPage from './pages/incidents/IncidentPage'
import CostPage from './pages/cost/CostPage'
import AuditPage from './pages/audit/AuditPage'
import SelfServicePage from './pages/selfservice/SelfServicePage'
import DevSecOpsPage from './pages/devsecops/DevSecOpsPage'
import TeamsPage from './pages/teams/TeamsPage'
import InfrastructurePage from './pages/infrastructure/InfrastructurePage'

const queryClient = new QueryClient({
  defaultOptions: { queries: { retry: 1, staleTime: 30000 } },
})

function PrivateRoute({ children }: { children: React.ReactNode }) {
  const token = useAuthStore((s) => s.token)
  return token ? <>{children}</> : <Navigate to="/login" replace />
}

export default function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />
          <Route path="/" element={<PrivateRoute><AppLayout /></PrivateRoute>}>
            <Route index element={<Navigate to="/dashboard" replace />} />
            <Route path="dashboard" element={<DashboardPage />} />
            <Route path="catalog" element={<CatalogPage />} />
            <Route path="self-service" element={<SelfServicePage />} />
            <Route path="cicd" element={<CicdPage />} />
            <Route path="kubernetes" element={<KubernetesPage />} />
            <Route path="observability" element={<ObservabilityPage />} />
            <Route path="gitops" element={<GitOpsPage />} />
            <Route path="vault" element={<VaultPage />} />
            <Route path="incidents" element={<IncidentPage />} />
            <Route path="cost" element={<CostPage />} />
            <Route path="audit" element={<AuditPage />} />
            <Route path="devsecops" element={<DevSecOpsPage />} />
            <Route path="teams" element={<TeamsPage />} />
            <Route path="infrastructure" element={<InfrastructurePage />} />
          </Route>
          <Route path="*" element={<Navigate to="/dashboard" replace />} />
        </Routes>
      </BrowserRouter>
    </QueryClientProvider>
  )
}
