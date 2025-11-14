interface Wager {
    wagerAmount: number;
    isPremiumToken: boolean;
}

interface DrawResult {
    value: number;
}

interface PlayResponse {
    wager: Wager;
    draw: DrawResult;
}

async function PlayFetch(
    wagerAmount: number,
    betType: number,
    selected: number[]
): Promise<{ ok: boolean; body: PlayResponse | string }> {
    const payload = {
        wager: { wagerAmount, isPremiumToken: false },
        rouletteChoice: { betType, selected },
    };

    return fetch('/api/games/roulette/play', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload),
    }).then(async response => {
        const data: PlayResponse | string = response.ok
            ? await response.json()
            : await response.text();

        return { ok: response.ok, body: data };
    });
}

// Alpine component typing
interface RouletteComponent {
    betType: number;
    numbersInput: string;
    wagerAmount: number;
    loading: boolean;
    result: PlayResponse | null;
    error: string;
    play: () => Promise<void>;
    rouletteGrid: () => NumberColor[];
}

// Factory used by Alpine: x-data="rouletteComponent()"
function rouletteComponent(initialBetType: number = 0): RouletteComponent {
    return {
        betType: initialBetType,
        numbersInput: '',
        wagerAmount: 10,
        loading: false,
        result: null,
        error: '',
        rouletteGrid: () => {
            const redNumbers = [1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36];

            const numbers: NumberColor[] = [];
            for (let num = 1; num <= 36; num++) {
                numbers.push({
                    value: num,
                    color: redNumbers.includes(num) ? 'red' : 'black'
                });
            }

            return numbers;
        },
        async play() {
            this.loading = true;
            this.error = '';
            this.result = null;

            try {
                const selected = this.numbersInput
                    .split(',')
                    .map(n => n.trim())
                    .filter(n => n !== '')
                    .map(n => parseInt(n, 10))
                    .filter(n => !Number.isNaN(n));

                const result = await PlayFetch(this.wagerAmount, this.betType, selected);

                if (!result.ok) {
                    this.error = `Error: ${result.body}`;
                    return;
                }

                this.result = result.body as PlayResponse;
            } catch (e: unknown) {
                const msg = e instanceof Error ? e.message : String(e);
                this.error = 'Unexpected error: ' + msg;
            } finally {
                this.loading = false;
            }
        },
    };
}
// Make rouletteComponent available globally for the Razor view / Alpine

interface NumberColor {
    value: number
    color: string
}

(window as unknown as {
    rouletteComponent: (initialBetType?: number) => RouletteComponent;
}).rouletteComponent = rouletteComponent;