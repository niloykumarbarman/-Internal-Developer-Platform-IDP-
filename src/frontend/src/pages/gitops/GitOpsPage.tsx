import { GitMerge, GitBranch, CheckCircle, Clock, ExternalLink, RefreshCw } from 'lucide-react'

const apps = [
  { name: 'enterprise-idp', repo: 'org/enterprise-idp', branch: 'main', env: 'Production', status: 'Synced', health: 'Healthy', lastSync: '5m ago' },
  { name: 'enterprise-idp-staging', repo: 'org/enterprise-idp', branch: 'staging', env: 'Staging', status: 'Synced', health: 'Healthy', lastSync: '12m ago' },
  { name: 'enterprise-idp-dev', repo: 'org/enterprise-idp', branch: 'develop', env: 'Development', status: 'OutOfSync', health: 'Degraded', lastSync: '2h ago' },
  { name: 'monitoring-stack', repo: 'org/infra-charts', branch: 'main', env: 'Platform', status: 'Synced', health: 'Healthy', lastSync: '1h ago' },
  { name: 'vault-cluster', repo: 'org/infra-charts', branch: 'main', env: 'Platform', status: 'Synced', health: 'Healthy', lastSync: '3h ago' },
]

const prs = [
  { id: 42, title: 'feat: add cost management dashboard', author: 'jane.smith', branch: 'feature/cost-ui', status: 'Open', checks: 'Passing', time: '2h ago' },
  { id: 41, title: 'fix: catalog service memory leak', author: 'john.doe', branch: 'fix/catalog-mem', status: 'Open', checks: 'Running', time: '5h ago' },
  { id: 40, title: 'chore: update helm chart values', author: 'ci-bot', branch: 'chore/helm-update', status: 'Merged', checks: 'Passing', time: '1d ago' },
  { id: 39, title: 'feat: vault secret rotation', author: 'bob.dev', branch: 'feature/vault-rotation', status: 'Merged', checks: 'Passing', time: '2d ago' },
]

const syncColor: Record<string, string> = { Synced: 'oklch(0.6 0.18 150)', OutOfSync: 'oklch(0.75 0.18 80)', Unknown: 'oklch(0.55 0 0)' }
const healthColor: Record<string, string> = { Healthy: 'oklch(0.6 0.18 150)', Degraded: 'oklch(0.75 0.18 80)', Missing: 'oklch(0.6 0.22 27)' }
const prColor: Record<string, string> = { Open: 'oklch(0.6 0.18 150)', Merged: 'oklch(0.6 0.2 250)', Closed: 'oklch(0.6 0.22 27)' }

export default function GitOpsPage() {
  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: '1.25rem' }}>
      <div>
        <h1 style={{ fontSize: 22, fontWeight: 700, color: 'oklch(0.95 0 0)', marginBottom: 4 }}>GitOps Platform</h1>
        <p style={{ fontSize: 13, color: 'oklch(0.5 0 0)' }}>ArgoCD · GitHub · App-of-Apps Pattern</p>
      </div>

      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(4, 1fr)', gap: '0.75rem' }}>
        {[
          { label: 'ArgoCD Apps', value: '5', color: 'oklch(0.75 0.18 80)' },
          { label: 'Synced', value: '4', color: 'oklch(0.6 0.18 150)' },
          { label: 'Out of Sync', value: '1', color: 'oklch(0.75 0.18 80)' },
          { label: 'Open PRs', value: '2', color: 'oklch(0.6 0.2 250)' },
        ].map(s => (
          <div key={s.label} style={{ background: 'oklch(0.18 0 0)', border: '1px solid oklch(0.28 0 0)', borderRadius: 8, padding: '1rem', textAlign: 'center' }}>
            <div style={{ fontSize: 26, fontWeight: 700, color: s.color }}>{s.value}</div>
            <div style={{ fontSize: 12, color: 'oklch(0.5 0 0)', marginTop: 4 }}>{s.label}</div>
          </div>
        ))}
      </div>

      {/* ArgoCD Apps */}
      <div style={{ background: 'oklch(0.18 0 0)', border: '1px solid oklch(0.28 0 0)', borderRadius: 10, padding: '1.25rem' }}>
        <div style={{ fontSize: 14, fontWeight: 600, color: 'oklch(0.85 0 0)', marginBottom: '1rem', display: 'flex', alignItems: 'center', gap: 8 }}>
          <GitMerge size={16} color="oklch(0.75 0.18 80)" /> ArgoCD Applications
        </div>
        <div style={{ display: 'flex', flexDirection: 'column', gap: 8 }}>
          {apps.map(app => (
            <div key={app.name} style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', padding: '0.75rem', background: 'oklch(0.21 0 0)', borderRadius: 8 }}>
              <div style={{ display: 'flex', alignItems: 'center', gap: 10 }}>
                <div style={{ width: 32, height: 32, borderRadius: 6, background: 'oklch(0.75 0.18 80 / 0.12)', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                  <GitMerge size={15} color="oklch(0.75 0.18 80)" />
                </div>
                <div>
                  <div style={{ fontSize: 13, fontWeight: 600, color: 'oklch(0.9 0 0)' }}>{app.name}</div>
                  <div style={{ fontSize: 11, color: 'oklch(0.5 0 0)' }}>{app.repo} · {app.branch} · {app.env}</div>
                </div>
              </div>
              <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
                <span style={{ fontSize: 12, color: 'oklch(0.5 0 0)' }}>{app.lastSync}</span>
                <span style={{ fontSize: 11, fontWeight: 600, padding: '2px 8px', borderRadius: 20, color: healthColor[app.health], background: `${healthColor[app.health].replace(')', ' / 0.12)')}` }}>{app.health}</span>
                <span style={{ fontSize: 11, fontWeight: 600, padding: '2px 8px', borderRadius: 20, color: syncColor[app.status], background: `${syncColor[app.status].replace(')', ' / 0.12)')}` }}>{app.status}</span>
                <button style={{ background: 'none', border: 'none', cursor: 'pointer', color: 'oklch(0.5 0 0)' }}><RefreshCw size={14} /></button>
              </div>
            </div>
          ))}
        </div>
      </div>

      {/* Pull Requests */}
      <div style={{ background: 'oklch(0.18 0 0)', border: '1px solid oklch(0.28 0 0)', borderRadius: 10, padding: '1.25rem' }}>
        <div style={{ fontSize: 14, fontWeight: 600, color: 'oklch(0.85 0 0)', marginBottom: '1rem', display: 'flex', alignItems: 'center', gap: 8 }}>
          <GitBranch size={16} color="oklch(0.6 0.2 250)" /> Pull Requests
        </div>
        <div style={{ display: 'flex', flexDirection: 'column', gap: 8 }}>
          {prs.map(pr => (
            <div key={pr.id} style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', padding: '0.75rem', background: 'oklch(0.21 0 0)', borderRadius: 8 }}>
              <div style={{ display: 'flex', alignItems: 'center', gap: 10 }}>
                <span style={{ fontSize: 12, color: 'oklch(0.5 0 0)', fontFamily: 'monospace' }}>#{pr.id}</span>
                <div>
                  <div style={{ fontSize: 13, fontWeight: 500, color: 'oklch(0.88 0 0)' }}>{pr.title}</div>
                  <div style={{ fontSize: 11, color: 'oklch(0.5 0 0)' }}>{pr.author} · {pr.branch} · {pr.time}</div>
                </div>
              </div>
              <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
                <span style={{ fontSize: 11, color: pr.checks === 'Passing' ? 'oklch(0.6 0.18 150)' : 'oklch(0.75 0.18 80)' }}>
                  {pr.checks === 'Passing' ? <CheckCircle size={13} /> : <Clock size={13} />}
                </span>
                <span style={{ fontSize: 11, fontWeight: 600, padding: '2px 8px', borderRadius: 20, color: prColor[pr.status], background: `${prColor[pr.status].replace(')', ' / 0.12)')}` }}>{pr.status}</span>
                <button style={{ background: 'none', border: 'none', cursor: 'pointer', color: 'oklch(0.5 0 0)' }}><ExternalLink size={13} /></button>
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  )
}
