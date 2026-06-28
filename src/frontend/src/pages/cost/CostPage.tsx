import { DollarSign, TrendingDown, TrendingUp, AlertTriangle } from 'lucide-react'

const teamCosts = [
  { team: 'Platform Team', current: 1240, budget: 1500, last: 1380, services: 8 },
  { team: 'Core Team', current: 680, budget: 800, last: 720, services: 5 },
  { team: 'Security Team', current: 320, budget: 400, last: 290, services: 3 },
  { team: 'Finance Team', current: 480, budget: 500, last: 510, services: 4 },
  { team: 'Frontend Team', current: 120, budget: 200, last: 140, services: 2 },
]

const k8sCosts = [
  { namespace: 'idp-prod', cpu: '$820', memory: '$340', storage: '$120', total: '$1,280' },
  { namespace: 'idp-staging', cpu: '$310', memory: '$180', storage: '$60', total: '$550' },
  { namespace: 'idp-dev', cpu: '$180', memory: '$95', storage: '$40', total: '$315' },
  { namespace: 'monitoring', cpu: '$240', memory: '$160', storage: '$80', total: '$480' },
  { namespace: 'vault', cpu: '$85', memory: '$65', storage: '$30', total: '$180' },
]

const Bar = ({ pct, color }: { pct: number; color: string }) => (
  <div style={{ height: 5, background: 'oklch(0.25 0 0)', borderRadius: 3, overflow: 'hidden', flex: 1 }}>
    <div style={{ height: '100%', width: `${Math.min(pct, 100)}%`, background: color, borderRadius: 3 }} />
  </div>
)

export default function CostPage() {
  const totalCurrent = teamCosts.reduce((a, t) => a + t.current, 0)
  const totalBudget = teamCosts.reduce((a, t) => a + t.budget, 0)
  const totalLast = teamCosts.reduce((a, t) => a + t.last, 0)
  const diff = totalCurrent - totalLast
  const diffPct = ((diff / totalLast) * 100).toFixed(1)

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: '1.25rem' }}>
      <div>
        <h1 style={{ fontSize: 22, fontWeight: 700, color: 'oklch(0.95 0 0)', marginBottom: 4 }}>Cost Management</h1>
        <p style={{ fontSize: 13, color: 'oklch(0.5 0 0)' }}>Resource Usage · Team Reports · Budget Alerts</p>
      </div>

      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(4, 1fr)', gap: '0.75rem' }}>
        {[
          { label: 'Monthly Spend', value: `$${totalCurrent.toLocaleString()}`, sub: `${diff > 0 ? '↑' : '↓'} ${Math.abs(+diffPct)}% vs last month`, color: 'oklch(0.6 0.2 250)', icon: DollarSign },
          { label: 'Budget Used', value: `${((totalCurrent / totalBudget) * 100).toFixed(0)}%`, sub: `$${totalBudget - totalCurrent} remaining`, color: 'oklch(0.6 0.18 150)', icon: TrendingDown },
          { label: 'vs Last Month', value: `${diff > 0 ? '+' : ''}$${diff}`, sub: diff < 0 ? 'Cost reduced ✓' : 'Cost increased', color: diff < 0 ? 'oklch(0.6 0.18 150)' : 'oklch(0.6 0.22 27)', icon: diff < 0 ? TrendingDown : TrendingUp },
          { label: 'Budget Alerts', value: '1', sub: 'Finance Team near limit', color: 'oklch(0.75 0.18 80)', icon: AlertTriangle },
        ].map(s => (
          <div key={s.label} style={{ background: 'oklch(0.18 0 0)', border: '1px solid oklch(0.28 0 0)', borderRadius: 8, padding: '1rem' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 8 }}>
              <div style={{ fontSize: 12, color: 'oklch(0.55 0 0)' }}>{s.label}</div>
              <s.icon size={16} color={s.color} />
            </div>
            <div style={{ fontSize: 22, fontWeight: 700, color: s.color }}>{s.value}</div>
            <div style={{ fontSize: 11, color: 'oklch(0.5 0 0)', marginTop: 4 }}>{s.sub}</div>
          </div>
        ))}
      </div>

      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem' }}>
        {/* Team Cost */}
        <div style={{ background: 'oklch(0.18 0 0)', border: '1px solid oklch(0.28 0 0)', borderRadius: 10, padding: '1.25rem' }}>
          <div style={{ fontSize: 14, fontWeight: 600, color: 'oklch(0.85 0 0)', marginBottom: '1rem' }}>Team Cost Reports</div>
          <div style={{ display: 'flex', flexDirection: 'column', gap: '0.85rem' }}>
            {teamCosts.map(t => {
              const pct = (t.current / t.budget) * 100
              const color = pct > 90 ? 'oklch(0.6 0.22 27)' : pct > 75 ? 'oklch(0.75 0.18 80)' : 'oklch(0.6 0.18 150)'
              return (
                <div key={t.team}>
                  <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 4 }}>
                    <div>
                      <span style={{ fontSize: 13, fontWeight: 500, color: 'oklch(0.85 0 0)' }}>{t.team}</span>
                      <span style={{ fontSize: 11, color: 'oklch(0.5 0 0)', marginLeft: 6 }}>{t.services} services</span>
                    </div>
                    <div style={{ textAlign: 'right' }}>
                      <span style={{ fontSize: 13, fontWeight: 700, color }}>${t.current}</span>
                      <span style={{ fontSize: 11, color: 'oklch(0.45 0 0)' }}> / ${t.budget}</span>
                    </div>
                  </div>
                  <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
                    <Bar pct={pct} color={color} />
                    <span style={{ fontSize: 11, color, minWidth: 36 }}>{pct.toFixed(0)}%</span>
                  </div>
                </div>
              )
            })}
          </div>
        </div>

        {/* K8s Cost by Namespace */}
        <div style={{ background: 'oklch(0.18 0 0)', border: '1px solid oklch(0.28 0 0)', borderRadius: 10, overflow: 'hidden' }}>
          <div style={{ padding: '1rem 1.25rem', borderBottom: '1px solid oklch(0.25 0 0)', fontSize: 14, fontWeight: 600, color: 'oklch(0.85 0 0)' }}>K8s Cost by Namespace</div>
          <table style={{ width: '100%', borderCollapse: 'collapse' }}>
            <thead>
              <tr style={{ borderBottom: '1px solid oklch(0.25 0 0)' }}>
                {['Namespace', 'CPU', 'Memory', 'Storage', 'Total'].map(h => (
                  <th key={h} style={{ padding: '0.6rem 0.85rem', textAlign: 'left', fontSize: 11, fontWeight: 600, color: 'oklch(0.45 0 0)', textTransform: 'uppercase' }}>{h}</th>
                ))}
              </tr>
            </thead>
            <tbody>
              {k8sCosts.map(k => (
                <tr key={k.namespace} style={{ borderBottom: '1px solid oklch(0.22 0 0)' }}>
                  <td style={{ padding: '0.65rem 0.85rem', fontSize: 12, fontFamily: 'monospace', color: 'oklch(0.8 0 0)' }}>{k.namespace}</td>
                  <td style={{ padding: '0.65rem 0.85rem', fontSize: 12, color: 'oklch(0.6 0 0)' }}>{k.cpu}</td>
                  <td style={{ padding: '0.65rem 0.85rem', fontSize: 12, color: 'oklch(0.6 0 0)' }}>{k.memory}</td>
                  <td style={{ padding: '0.65rem 0.85rem', fontSize: 12, color: 'oklch(0.6 0 0)' }}>{k.storage}</td>
                  <td style={{ padding: '0.65rem 0.85rem', fontSize: 12, fontWeight: 700, color: 'oklch(0.6 0.2 250)' }}>{k.total}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  )
}
