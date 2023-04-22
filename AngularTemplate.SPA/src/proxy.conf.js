const PROXY_CONFIG = [
	{
		context: [
			'/api',
			'/authorize',
			'/.well-known',
			'/connect',
			'/weatherforecast',
		],
		target: 'https://localhost:9101',
		secure: false
	}
];

module.exports = PROXY_CONFIG;
