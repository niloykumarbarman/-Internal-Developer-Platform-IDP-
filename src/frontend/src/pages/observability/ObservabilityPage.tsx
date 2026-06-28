import { Activity, AlertTriangle, BarChart3, Eye, Server, Wifi } from 'lucide-react'

const metrics = [
  { service: 'api-gateway', rps: '1,240', p99: '45ms', errors: '0.2%', uptime: '99.98%' },
  { service: 'auth-service', rps: '890', p99: '22ms', errors: '0.0%', uptime: '100%' },
  { service: 'catalog-service', rps: '340', p99: '120ms', errors: '1.4%', uptime: '99.71%' },
  { service: 'payment-service', rps: '210', p99: '88ms', errors: '0.1%', uptime: '99.95%' },
  { service: 'notification-svc', rps: '56', p99: '200ms', errors: '2.1%', uptime: '99.50%' },
]

const alerts = [
  { id: '1', name: 'High Error Rate', service: 'catalog-service', severity: 'Warning', time: '5m ago', status: 'Firing' },
  { id: '2', name: 'High Latency P99', service: 'notification-svc', severity: 'Warning', time: '12m ago', status: 'Firing' },
  { id: '3', name: 'Pod CrashLoop', service: 'notif-svc', severity: 'Critical', time: '6h ago', status: 'Firing' },
  { id: '4', name: 'CPU Throttling', service: 'catalog-service', severity: 'Info', time: '1h ago', status: 'Resolved' },
]

const sevColor: Record<string, string> = { Critical: 'oklch(0.6 0.22 27)', Warning: 'oklch(0.75 0.18 80)', Info: 'oklch(0.6 0.2 250)' }

const Bar = ({ pct, color }: { pct: number; color: string }) => (
  <div style={{ height: 6, background: 'oklch(0.25 0 0)', borderRadius: 3, overflow: 'hidden' }}>
    <div style={{ height: '100%', width: `${pct}%`, background: color, borderRadius: 3, transition: 'width 0.5s' }} />
  </div>
)

export default function ObservabilityPage() {
  const tools = [
    { name: 'Prometheus', status: 'Healthy', url: 'http://prometheus:9090', icon: BarChart3, color: 'oklch(0.75 0.18 50)' },
    { name: 'Grafana', status: 'Healthy', url: 'http://grafana:3000', icon: Activity, color: 'oklch(0.75 0.18 80)' },
    { name: 'Loki', status: 'Healthy', url: 'http://loki:3100', icon: Eye, color: 'oklch(0.6 0.18 150)' },
    { name: 'Jaeger', status: 'Healthy', url: 'http://jaeger:16686', icon: Wifi, color: 'oklch(0.6 0.2 250)' },
    { name: 'Alertmanager', status: 'Healthy', url: 'http://alertmanager:9093', icon: AlertTriangle, color: 'oklch(0.6 0.22 27)' },
    { name: 'OpenTelemetry', status: 'Healthy', url: 'http://otel:4317', icon: Server, color: 'oklch(0.75 0.18 200)' },
  ]

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: '1.25rem' }}>
      <div>
        <h1 style={{ fontSize: 22, fontWeight: 700, color: 'oklch(0.95 0 0)', marginBottom: 4 }}>Observability Platform</h1>
        <p style={{ fontSize: 13, color: 'oklch(0.5 0 0)' }}>Prometheus · Grafana · Loki · Jaeger · OpenTelemetry</p>
      </div>

      {/* Tool Status */}
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(3, 1fr)', gap: '0.75rem' }}>
        {tools.map(t => (
          <div key={t.name} style={{ background: 'oklch(0.18 0 0)', border: '1px solid oklch(0.28 0 0)', borderRadius: 8, padding: '0.85rem', display: 'flex', alignItems: 'center', gap: 10 }}>
            <div style={{ width: 36, height: 36, borderRadius: 8, background: `${t.color.replace(')', ' / 0.12)')}`, display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
              <t.icon size={18} color={t.color} />
            </div>
            <div style={{ flex: 1 }}>
              <div style={{ fontSize: 13, fontWeight: 600, color: 'oklch(0.9 0 0)' }}>{t.name}</div>
              <div style={{ fontSize: 11, color: 'oklch(0.5 0 0)' }}>{t.url}</div>
            </div>
            <span style={{ fontSize: 11, fontWeight: 600, padding: '2px 8px', borderRadius: 20, color: 'oklch(0.6 0.18 150)', background: 'oklch(0.6 0.18 150 / 0.12)' }}>{t.status}</span>
          </div>
        ))}
      </div>

      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem' }}>
        {/* Service Metrics */}
        <div style={{ background: 'oklch(0.18 0 0)', border: '1px solid oklch(0.28 0 0)', borderRadius: 10, padding: '1.25rem' }}>
          <div style={{ fontSize: 14, fontWeight: 600, color: 'oklch(0.85 0 0)', marginBottom: '1rem' }}>Service Metrics</div>
          <table style={{ width: '100%', borderCollapse: 'collapse' }}>
            <thead>
              <tr>
                {['Service', 'RPS', 'P99', 'Errors', 'Uptime'].map(h => (
                  <th key={h} style={{ padding: '0.4rem 0.5rem', textAlign: 'left', fontSize: 11, color: 'oklch(0.45 0 0)', fontWeight: 600, textTransform: 'uppercase' }}>{h}</th>
                ))}
              </tr>
            </thead>
            <tbody>
              {metrics.map(m => (
                <tr key={m.service} style={{ borderTop: '1px solid oklch(0.22 0 0)' }}>
                  <td style={{ padding: '0.5rem', fontSize: 12, color: 'oklch(0.8 0 0)', fontWeight: 500 }}>{m.service}</td>
                  <td style={{ padding: '0.5rem', fontSize: 12, color: 'oklch(0.6 0.2 250)' }}>{m.rps}</td>
                  <td style={{ padding: '0.5rem', fontSize: 12, color: 'oklch(0.65 0 0)' }}>{m.p99}</td>
                  <td style={{ padding: '0.5rem', fontSize: 12, color: parseFloat(m.errors) > 1 ? 'oklch(0.6 0.22 27)' : 'oklch(0.6 0.18 150)' }}>{m.errors}</td>
                  <td style={{ padding: '0.5rem', fontSize: 12, color: 'oklch(0.6 0.18 150)' }}>{m.uptime}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>

        {/* Alerts */}
        <div style={{ background: 'oklch(0.18 0 0)', border: '1px solid oklch(0.28 0 0)', borderRadius: 10, padding: '1.25rem' }}>
          <div style={{ fontSize: 14, fontWeight: 600, color: 'oklch(0.85 0 0)', marginBottom: '1rem', display: 'flex', justifyContent: 'space-between' }}>
            Active Alerts <span style={{ fontSize: 12, color: 'oklch(0.6 0.22 27)', fontWeight: 700 }}>{alerts.filter(a => a.status === 'Firing').length} firing</span>
          </div>
          <div style={{ display: 'flex', flexDirection: 'column', gap: 8 }}>
            {alerts.map(a => (
              <div key={a.id} style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', padding: '0.6rem 0.75rem', background: 'oklch(0.21 0 0)', borderRadius: 6, borderLeft: `3px solid ${sevColor[a.severity]}` }}>
                <div>
                  <div style={{ fontSize: 13, fontWeight: 500, color: 'oklch(0.88 0 0)' }}>{a.name}</div>
                  <div style={{ fontSize: 11, color: 'oklch(0.5 0 0)' }}>{a.service} · {a.time}</div>
                </div>
                <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'flex-end', gap: 3 }}>
                  <span style={{ fontSize: 11, fontWeight: 600, color: sevColor[a.severity] }}>{a.severity}</span>
                  <span style={{ fontSize: 10, color: a.status === 'Firing' ? 'oklch(0.6 0.22 27)' : 'oklch(0.6 0.18 150)' }}>{a.status}</span>
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>

      {/* Resource Usage Bars */}
      <div style={{ background: 'oklch(0.18 0 0)', border: '1px solid oklch(0.28 0 0)', borderRadius: 10, padding: '1.25rem' }}>
        <div style={{ fontSize: 14, fontWeight: 600, color: 'oklch(0.85 0 0)', marginBottom: '1rem' }}>Cluster Resource Usage</div>
        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(3, 1fr)', gap: '1.25rem' }}>
          {[
            { label: 'CPU Usage', value: 62, color: 'oklch(0.75 0.18 80)' },
            { label: 'Memory Usage', value: 71, color: 'oklch(0.6 0.2 250)' },
            { label: 'Disk Usage', value: 45, color: 'oklch(0.6 0.18 150)' },
          ].map(r => (
            <div key={r.label}>
              <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 6 }}>
                <span style={{ fontSize: 13, color: 'oklch(0.7 0 0)' }}>{r.label}</span>
                <span style={{ fontSize: 13, fontWeight: 700, color: r.color }}>{r.value}%</span>
              </div>
              <Bar pct={r.value} color={r.color} />
            </div>
          ))}
        </div>
      </div>
    </div>
  )
}
